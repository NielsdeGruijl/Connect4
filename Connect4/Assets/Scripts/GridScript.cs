using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    [Header("Grid Content")]
    [SerializeField] private GameObject node;
    private List<Node> nodes = new List<Node>();

    [Header("Grid values")]
    [SerializeField] private Vector2Int cellSize;
    [SerializeField] private int rowLength, colLength;
    private int gridWidth, gridHeight;

    [HideInInspector] public List<Vector3> coinSpawnPositions = new List<Vector3>();
    
    private void Start()
    {
        //the row and column at position 0 shouldn't be accounted for in the "grid size"
        gridWidth = (rowLength - 1) * cellSize.x;
        gridHeight = (colLength - 1) * cellSize.y;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        int nodeID = 0;
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
                
                nodeID++;

                //Add positions above each column to spawn coins for the fall animation
                if (i == colLength - 1)
                {
                    coinSpawnPositions.Add(new Vector3(cellPos.x, cellPos.y + cellSize.y, 0));
                }
            }
        }

        //once the grid has been built, assign sort the neighbours based on direction for each node 
        foreach (Node node in nodes)
        {
            AssignNeighbours(node);
            node.Sortneighbours();
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

            //find and store all neighbours
            if ((nodes[i].pos - currentNode.pos).magnitude < diagonalDistance)
            {
                currentNode.neighbours.Add(nodes[i]);
            }
        }
    }
    
    public Node GetLowestAvailableNode(Node node)
    {
        int nodeID = node.id;
        int lowestNodeID;

        //get the ID of the lowest node in the column
        while (nodeID >= rowLength)
        {
            nodeID -= rowLength;
        }

        //check every node in the column starting at the bottom
        for (int i = 0; i < colLength; i++)
        {
            //move up a row, nodeID determines the column and i * rowLength determines the row
            lowestNodeID = nodeID + (i * rowLength);
            
            if (nodes[lowestNodeID].occupied)
                continue;
            else
                return nodes[lowestNodeID];
        }

        Debug.Log("Column is full");
        return null;
    }

    public bool ConnectFour(Node node, Node.direction dir, int playerID)
    {
        bool otherDirChecked = false;
        int totalConnected = 1;
        Node.direction tempDir = dir;
        Node tempNode = node;
        Node neighbour;

        while (totalConnected < 4)
        {
            neighbour = tempNode.CheckNeighbour(tempDir, playerID);
            //if the neighbour contains a coin of the current player
            if (neighbour != null)
            {
                totalConnected++;
                tempNode = neighbour;
            }
            //if we haven't checked the opposite direction yet
            else if (!otherDirChecked)
            {
                //reset tempNode
                tempNode = node;

                //get the opposite direction
                int direction = (int)dir;
                direction += 4;

                //set the opposite direction
                tempDir = (Node.direction)direction;

                //Debug.Log(tempDir);

                otherDirChecked = true;
            }
            //no connect 4 has been found
            else
                return false;
        }

        //a connect 4 has been found
        return true;
    }

    public int FindColumn(int nodeID)
    {
        if(nodeID == 0)
            return 0;
        
        return nodeID % rowLength;
    }
}
