using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinPlacer : MonoBehaviour
{
    [Header("Game Board")]
    [SerializeField] private GameObject coin1, coin2;
    [SerializeField] private GridScript grid;

    [Header("Player Components")]
    [SerializeField] private LayerMask layerMask;

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
                Node availableNode = grid.GetLowestAvailableNode(node);
                //if column is full, return
                if (availableNode == null) return;

                if (turnManager.getPlayerID == TurnManager.player1)
                    Instantiate(coin1, availableNode.transform.position, coin1.transform.rotation);
                else
                    Instantiate(coin2, availableNode.transform.position, coin2.transform.rotation);

                availableNode.occupied = true;
                availableNode.ownerID = turnManager.getPlayerID;
                CheckConnectFour(availableNode, turnManager.getPlayerID);

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
