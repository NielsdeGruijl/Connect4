using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Vector2 pos;
    public Vector2 Position { get { return pos; } }
    private int id;
    public int ID { get { return id; } }


    public List<Node> neighbours = new List<Node>();

    public bool player1 = false;
    public bool player2 = false;

    public void Initialize(int id, Vector2 pos)
    {
        this.id = id;
        this.pos = pos;
    }

    public void CheckNeighbours()
    {
        foreach(Node node in neighbours)
        {

        }
    }
}
