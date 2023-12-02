using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

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
    [SerializeField] private UIManager uiManager;

    //External components
    private Camera cam;
    private TurnManager turnManager;

    //Coin variables
    private CoinScript coinScript;
    private int columnPosition;
    private float animLength;

    private bool gameWon = false;

    private void Start()
    {
        input = GetComponent<InputScript>();
        turnManager = GetComponent<TurnManager>();
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
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
        Debug.Log(columnPosition);
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
        availableNode.ownerID = turnManager.PlayerID;
        CheckConnectFour(availableNode, turnManager.PlayerID);

        animLength = coinScript.animationLength;
        coinScript = null;

        if (!gameWon)
            turnManager.ChangeTurns(animLength);
        else
            turnManager.StopAllCoroutines();
    }

    private void CheckConnectFour(Node node, int playerID)
    {
        //Check for 4 connected coins along each possible axis (ConnectFour checks both the given and opposite direction)
        if (grid.ConnectFour(node, Node.direction.topLeft, playerID) || grid.ConnectFour(node, Node.direction.top, playerID) ||
           grid.ConnectFour(node, Node.direction.topRight, playerID) || grid.ConnectFour(node, Node.direction.right, playerID))
        {
            gameWon = true;
            StartCoroutine(showWinTextCo(playerID));
        }
    }

    private IEnumerator showWinTextCo(int playerID)
    {
        yield return new WaitForSeconds(animLength);
        input.inputLocked = true;
        uiManager.DisplayWinText(playerID);
    }
}
