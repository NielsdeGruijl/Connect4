using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    [SerializeField] private GameObject node;
    private List<Node> nodes = new List<Node>();

    [Header("Grid values")]
    [SerializeField] private Vector2Int cellSize;
    [SerializeField] private int rowLength, colLength;
    
    private int gridWidth, gridHeight;
    
    private int nodeID = 0;
    
    private void Start()
    {
        //the row and column at position 0 shouldn't be accounted for in the "grid size"
        gridWidth = (rowLength - 1) * cellSize.x;
        gridHeight = (colLength - 1) * cellSize.y;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        //rows
        for(int i = 0; i < colLength;  i++)
        {
            //colums
            for(int j = 0; j < rowLength; j++)
            {
                //set the node position
                int xPosition = -gridWidth/2 + (cellSize.x * j);
                int yPosition = 0 + (cellSize.y * i);
                Vector2 cellPos = new Vector2(xPosition, yPosition);

                //initialize the node
                GameObject nodeObject = Instantiate(node, cellPos, Quaternion.identity);
                nodeObject.transform.parent = transform;
                nodeObject.transform.name = $"Node {nodeID}";
                Node nodeScript = nodeObject.GetComponent<Node>();
                nodeScript.Initialize(nodeID, cellPos);
                nodes.Add(nodeScript);
                AssignNeighbours(nodeScript);
                nodeID++;
            }
        }
    }

    private void AssignNeighbours(Node currentNode)
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            //current node can't be a neighbour to itself
            if (nodes[i] == currentNode) continue;

            //calculate distance to any diagonal neighbour plus a 0.1f margin
            float diagonalDistance = Mathf.Sqrt(Mathf.Pow(cellSize.x, 2) + Mathf.Pow(cellSize.y, 2)) + 0.1f;

            //add neighbours to list and current node to the neighbours' list
            if ((nodes[i].Position - currentNode.Position).magnitude < diagonalDistance)
            {
                currentNode.neighbours.Add(nodes[i]);
                nodes[i].neighbours.Add(currentNode);
            }
        }
    }

    public Node GetLowestAvailableNode(Node node)
    {
        int nodeID = node.ID;
        int lowestNodeID;

        //get the ID of the lowest node in the column
        while (nodeID >= 7)
        {
            nodeID -= 7;
        }

        //check every node in the column starting at the bottom
        for (int i = 0; i < colLength; i++)
        {
            //move up a row, nodeID determines the column and i * rowLength determines the row
            lowestNodeID = nodeID + (i * rowLength);
            //if the node is occupied, continue, otherwise return available node
            if (nodes[lowestNodeID].player1 || nodes[lowestNodeID].player2)
                continue;
            else
                return nodes[lowestNodeID];
        }

        Debug.Log("Column is full");
        return null;
    }
}
