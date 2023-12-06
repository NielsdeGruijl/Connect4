using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPlacer : MonoBehaviour
{
    [Header("Game Board")]
    [SerializeField] private GridScript grid;
    [SerializeField] private GameObject coin1;
    [SerializeField] private GameObject coin2;
    [SerializeField] private Transform coinParent;

    [Header("Player Components")]
    [SerializeField] private LayerMask layerMask;
    private InputScript input;

    [Header("UI")]
    [SerializeField] private GameManagerScript connect4Handler;

    //External components
    private TurnManager turnManager;

    //Coin variables
    private CoinScript coinScript;
    private int columnPosition;
    private float animLength;

    public bool gameWon = false; //========== FIX THIS SHIT ==============

    private void Start()
    {
        input = GetComponent<InputScript>();
        turnManager = GetComponent<TurnManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SpawnCoin(int playerID)
    {
        //set column position to middle column by default
        columnPosition = Mathf.FloorToInt(grid.GridSize.x / 2) - 1;

        //spawn a coin above the middle column of the grid
        if (playerID == TurnManager.player1)
            coinScript = Instantiate(coin1, coinParent).GetComponent<CoinScript>();
        else
            coinScript = Instantiate(coin2, coinParent).GetComponent<CoinScript>();

        grid.placedCoins.Add(coinScript.gameObject);
        MoveCoinToColumn(1);
    }

    public void DestroyCoin()
    {
        if(coinScript != null)
        {
            Destroy(coinScript.gameObject);
            coinScript = null;
        }
    }

    public void MoveCoinToColumn(int direction)
    {
        int lastColumnPos = columnPosition;
        columnPosition += direction;
        columnPosition = Mathf.Clamp(columnPosition, 0, grid.GridSize.x - 1);
        if(coinScript != null && columnPosition != lastColumnPos)
            coinScript.transform.localPosition = grid.coinSpawnPositions[grid.FindColumn(columnPosition)];
    }

    public void LockInColumn()
    {
        Node availableNode = grid.GetLowestAvailableNode(columnPosition);

        //if column is full, return
        if (availableNode == null) return;

        coinScript.DropCoin(availableNode.transform.localPosition);

        availableNode.occupied = true;
        availableNode.ownerID = TurnManager.playerID;
        CheckConnect4(availableNode, TurnManager.playerID);

        animLength = coinScript.animationLength;
        coinScript = null;

        if (!gameWon)
            turnManager.ChangeTurns(animLength);
        else
            turnManager.StopAllCoroutines();
    }

    private void CheckConnect4(Node node, int playerID)
    {
        List<Node> connectedNodes = new List<Node>();

        //Check for 4 connected coins along each possible axis (grid.FindConnect4 checks both the given and opposite direction)
        for(int i = 0; i < 4; i++)
        {
            //get the direction enum by integer and check that axis
            Node.direction dir = (Node.direction)i;
            connectedNodes = grid.FindConnect4(node, dir, playerID);

            if(connectedNodes.Count >= 4)
            {
                gameWon = true;
                StartCoroutine(showWinTextCo(playerID, connectedNodes));
                break;
            }
        }
    }

    private IEnumerator showWinTextCo(int playerID, List<Node> connectedNodes)
    {
        yield return new WaitForSeconds(animLength);
        input.inputLocked = true;
        connect4Handler.HandleConnect4(playerID, connectedNodes);
    }
}
