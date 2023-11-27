using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPlacer : MonoBehaviour
{
    [SerializeField] private GameObject coin1, coin2;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GridScript grid;
    [SerializeField] private GameObject p1WinText, p2WinText;
    private Camera cam;
    private TurnManager turnManager;

    private bool player1Wins = false;
    private bool player2Wins = false;

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectPosition();
        }
    }

    private void SelectPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, 100, layerMask))
        {
            if(hit.transform.TryGetComponent<Node>(out Node node))
            {
                Node availableNode = grid.GetLowestAvailableNode(node);
                //if column is full, return
                if (availableNode == null) return;

                if (turnManager.player1Turn)
                {
                    Instantiate(coin1, availableNode.transform.position, coin1.transform.rotation);
                    availableNode.player1 = true;
                    CheckConnectFour(availableNode, 0);
                }
                else if(turnManager.player2Turn)
                {
                    Instantiate(coin2, availableNode.transform.position, coin2.transform.rotation);
                    availableNode.player2 = true;
                    CheckConnectFour(availableNode, 1);
                }
                turnManager.ChangeTurns();
            }
        }
        else
        {
            Debug.Log("Nothing was hit");
        }
    }

    private void CheckConnectFour(Node node, int playerID)
    {
        if (grid.ConnectFour(node, Node.direction.topLeft, playerID) || grid.ConnectFour(node, Node.direction.top, playerID) ||
           grid.ConnectFour(node, Node.direction.topRight, playerID) || grid.ConnectFour(node, Node.direction.right, playerID))
        {
            if (playerID == 0)
            {
                player1Wins = true;
                p1WinText.SetActive(true);
                //Debug.Log("PLAYER 1 WINS");
            }
            if (playerID == 1)
            {
                player2Wins = true;
                p2WinText.SetActive(true);
                //Debug.Log("PLAYER 2 WINS");
            }
        }
        else
            Debug.Log("No connect 4 found");            
    }
}
