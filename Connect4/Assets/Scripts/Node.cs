using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum direction
    { 
        topLeft,
        top,
        topRight,
        right,
        bottomRight,
        bottom,
        bottomLeft,
        left
    }

    private Vector2 pos;
    public Vector2 getPosition { get { return pos; } }
    private int id;
    public int getID { get { return id; } }

    public List<Node> neighbours = new List<Node>();

    public bool player1 = false;
    public bool player2 = false;

    private Dictionary<direction, Node> directionalNeighbours = new Dictionary<direction, Node>();

    public void Initialize(int id, Vector2 pos)
    {
        this.id = id;
        this.pos = pos;
    }

    public void Sortneighbours()
    {
        foreach (Node node in neighbours)
        {
            //check if the neighbour is above (topLeft, top, topRight)
            if(node.getPosition.y > pos.y)
            {
                if(node.getPosition.x < pos.x)
                    directionalNeighbours.Add(direction.topLeft, node);
                else if(node.getPosition.x > pos.x)
                    directionalNeighbours.Add(direction.topRight, node);
                else
                    directionalNeighbours.Add(direction.top, node);
            }

            //check if the neighbour is to the right
            if(node.getPosition.x > pos.x && node.getPosition.y == pos.y)
            {
                directionalNeighbours.Add(direction.right, node);
            }
                

            //check if the neighbour is below (bottomLeft, bottom, bottomRight)
            if(node.getPosition.y < pos.y)
            {
                if(node.getPosition.x > pos.x)
                    directionalNeighbours.Add(direction.bottomRight, node);
                else if(node.getPosition.x < pos.x)
                    directionalNeighbours.Add(direction.bottomLeft, node);
                else
                    directionalNeighbours.Add(direction.bottom, node);
            }

            //check if the neighbour is to the left
            if(node.getPosition.x < pos.x && node.getPosition.y == pos.y)
                directionalNeighbours.Add(direction.left, node);
        }
    }

    public Node CheckNeighbour(direction dir, int playerID)
    {
        if (!directionalNeighbours.ContainsKey(dir)) return null;
        
        if (directionalNeighbours[dir].player1 && playerID == 0)
        {
            return directionalNeighbours[dir];
        }
        if (directionalNeighbours[dir].player2 && playerID == 1)
        {
            return directionalNeighbours[dir];
        }
        else
        {
            return null;
        }
    }
}
