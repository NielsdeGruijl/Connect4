using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class GridScript : MonoBehaviour
{
    Camera cam;

    [Header("Grid Content")]
    [SerializeField] private Transform nodeParent;
    [SerializeField] private GameObject node;
    private List<Node> nodes = new List<Node>();

    [Header("Grid values")]
    [SerializeField] private Vector2Int cellSize;
    [SerializeField] public int rowLength;
    [SerializeField] public int colLength;
    private int gridWidth, gridHeight;
    public Vector2Int GridSize { get; private set; }

    [Header("Multipliers")]
    [SerializeField] private float maxMultipliers;
    private float chanceToAddMultiplier = 0.1f;
    private List<Node> nodesWithMultipliers = new List<Node>();

    [Header("Node UI")]
    [SerializeField] GameObject nodeUI;
    [SerializeField] Transform nodeUIParent;

    [HideInInspector] public List<Vector3> coinSpawnPositions = new List<Vector3>();
    [HideInInspector] public List<GameObject> placedCoins = new List<GameObject>();

    private void Awake()
    {
        GridSize = new Vector2Int(rowLength, colLength);

        //the row and column at position 0 shouldn't be accounted for in the "grid size"
        gridWidth = (rowLength - 1) * cellSize.x;
        gridHeight = (colLength - 1) * cellSize.y;

        cam = Camera.main;

        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ClearGrid();
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
                nodeObject.transform.parent = nodeParent;
                nodeObject.transform.name = $"Node {nodeID}";

                Node nodeScript = nodeObject.GetComponent<Node>();
                nodeScript.UIParent = nodeUIParent;
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

        SetMultipliers();
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
    
    public Node GetLowestAvailableNode(int nodeID)
    {
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
        
        return null;
    }

    public List<Node> FindConnect4(Node node, Node.direction dir, int playerID)
    {
        bool otherDirChecked = false;
        int totalConnected = 1;
        Node.direction tempDir = dir;
        Node tempNode = node;
        Node neighbour = node;

        List<Node> connectedNodes = new List<Node>
        {
            node
        };

        //keep checking until both directions have been explored
        while (!otherDirChecked || neighbour != null)
        {
            neighbour = tempNode.CheckNeighbour(tempDir, playerID);
            //if the neighbour contains a coin of the current player
            if (neighbour != null)
            {
                totalConnected++;
                tempNode = neighbour;

                connectedNodes.Add(neighbour);
            }
            //if we haven't checked the opposite direction yet
            else if (!otherDirChecked)
            {
                //reset tempNode
                tempNode = node;
                neighbour = node;

                //get the opposite direction
                int direction = (int)dir;
                direction += 4;

                //set the opposite direction
                tempDir = (Node.direction)direction;

                otherDirChecked = true;
            }
        }

        return connectedNodes;
    }

    public int FindColumn(int nodeID)
    {
        if(nodeID == 0)
            return 0;
        
        return nodeID % rowLength;
    }

    public void ClearGrid()
    {
        int totalCoins = placedCoins.Count - 1;
        for(int i = totalCoins; i >= 0; i--)
        {
            Destroy(placedCoins[i]);
        }

        placedCoins.Clear();

        foreach(Node node in nodes)
        {
            node.occupied = false;
        }

        SetMultipliers();
    }

    private void SetMultipliers()
    {
        if (nodesWithMultipliers.Count > 0)
        {
            foreach (Node node in nodesWithMultipliers)
                node.ResetMultiplier();
        }

        List<Node> tempNodes = new List<Node>(nodes);

        for(int i = 0; i < maxMultipliers; i++)
        {
            int nodeID = Random.Range(0, nodes.Count - i);
            Node node = tempNodes[nodeID];
            tempNodes.Remove(node);

            float multiplierVariantChance = 1f/3f;
            float chance = Random.Range(0f, 1f);
            float multiplier = 1;
            
            if (chance <= multiplierVariantChance)
                multiplier = 0.5f;
            else if (chance <= multiplierVariantChance * 2)
                multiplier = 1.5f;
            else
                multiplier = 2f;

            Debug.Log(nodeID);
            
            node.SetMultiplier(multiplier);
            nodesWithMultipliers.Add(node);
        }
    }
}
