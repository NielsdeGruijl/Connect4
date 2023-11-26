using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPlacer : MonoBehaviour
{
    [SerializeField] private GameObject coin1, coin2;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GridScript grid;
    private Camera cam;
    private TurnManager turnManager;

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
                }
                else if(turnManager.player2Turn)
                {
                    Instantiate(coin2, availableNode.transform.position, coin2.transform.rotation);
                    availableNode.player2 = true;
                }
                turnManager.ChangeTurns();
            }
        }
        else
        {
            Debug.Log("Nothing was hit");
        }
    }
}
