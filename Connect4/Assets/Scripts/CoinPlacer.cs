using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinPlacer : MonoBehaviour
{
    [Header("Game Board")]
    [SerializeField] private GridScript grid;
    [SerializeField] private GameObject coin1;
    [SerializeField] private GameObject coin2;

    [Header("Player Components")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private InputScript input;

    [Header("UI")]
    [SerializeField] private TMP_Text winText;

    //External components
    private Camera cam;
    private TurnManager turnManager;

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    public void SelectPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, 100, layerMask))
        {
            if(hit.transform.TryGetComponent<Node>(out Node node))
            {
                CoinScript coinScript;
                Node availableNode = grid.GetLowestAvailableNode(node);

                //if column is full, return
                if (availableNode == null) return;

                //get the position on top of the column to drop the coin from
                Vector3 spawnPos = grid.coinSpawnPositions[grid.FindColumn(availableNode.id)];

                if (turnManager.getPlayerID == TurnManager.player1)
                    coinScript = Instantiate(coin1, spawnPos, coin1.transform.rotation).GetComponent<CoinScript>();
                else
                    coinScript = Instantiate(coin2, spawnPos, coin2.transform.rotation).GetComponent<CoinScript>();
                
                coinScript.targetPos = availableNode.pos;

                availableNode.occupied = true;
                availableNode.ownerID = turnManager.getPlayerID;
                CheckConnectFour(availableNode, turnManager.getPlayerID);

                turnManager.ChangeTurns(coinScript.animationLength);
            }
        }
        else
        {
            Debug.Log("Nothing was hit");
        }
    }

    private void CheckConnectFour(Node node, int playerID)
    {
        //Check for 4 connected coins along each possible axis (ConnectFour checks both the given and opposite direction)
        if (grid.ConnectFour(node, Node.direction.topLeft, playerID) || grid.ConnectFour(node, Node.direction.top, playerID) ||
           grid.ConnectFour(node, Node.direction.topRight, playerID) || grid.ConnectFour(node, Node.direction.right, playerID))
        {
            winText.gameObject.SetActive(true);
            winText.text = $"PLAYER {playerID + 1} WINS!";
        }
        else
            Debug.Log("No connect 4 found");            
    }
}
