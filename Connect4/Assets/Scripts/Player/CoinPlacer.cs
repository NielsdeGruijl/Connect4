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

    [Header("External Components")]
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private LayerMask layerMask;

    //Coin variables
    private CoinScript coinScript;
    private int columnPosition;

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

    //move the coin to the column left or right of the current one based on player input
    public void MoveCoinToColumn(int direction)
    {
        int lastColumnPos = columnPosition;
        columnPosition += direction;
        columnPosition = Mathf.Clamp(columnPosition, 0, grid.GridSize.x - 1);
        if(coinScript != null && columnPosition != lastColumnPos)
            coinScript.transform.localPosition = grid.coinSpawnPositions[grid.FindColumn(columnPosition)];
    }

    //Place the coin in the column it's hovering above
    public void LockInColumn()
    {
        Node availableNode = grid.GetLowestAvailableNode(columnPosition);

        //if column is full, return
        if (availableNode == null) return;

        coinScript.DropCoin(availableNode.transform.localPosition);

        availableNode.SetOccupied(true);
        availableNode.ownerID = TurnManager.playerID;
        
        gameManager.CheckConnect4(availableNode, TurnManager.playerID, coinScript.animationLength);
        
        coinScript = null;
    }
}
