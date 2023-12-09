using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    //all possible directions for neighbours
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

    //Readonly vars
    public Vector2 pos { get; private set; }
    public int id { get; private set; }
    public bool occupied { get; private set; }

    [Header("Neighbours")]
    public List<Node> neighbours = new List<Node>();
    private Dictionary<direction, Node> directionalNeighbours = new Dictionary<direction, Node>();

    [Header("Owning player")]
    public int ownerID;

    [Header("Multiplier")]
    public float multiplier;

    [Header("UI elements")]
    [SerializeField] private GameObject multiplierUI;
    private TMP_Text multiplierText;

    [Header("particles")]
    [SerializeField] ParticleSystem particles;

    //Initializes variables of the node, called when the node is created
    public void Initialize(int id, Vector2 pos, Transform UIParent)
    {
        GameObject uiObject = Instantiate(multiplierUI, UIParent);
        multiplierText = uiObject.GetComponent<TMP_Text>();
        multiplierText.transform.position = new Vector3(transform.position.x, transform.position.y, UIParent.transform.position.z);
        multiplierText.enabled = false;

        this.id = id;
        this.pos = pos;
        multiplier = 1f;
    }

    public void Sortneighbours()
    {
        //add each neighbour with the correct direction as key to the dictionary for easier access later on
        foreach (Node node in neighbours)
        {
            //check if the neighbour is above (topLeft, top, topRight)
            if(node.pos.y > pos.y)
            {
                if(node.pos.x < pos.x)
                    directionalNeighbours.Add(direction.topLeft, node);
                else if(node.pos.x > pos.x)
                    directionalNeighbours.Add(direction.topRight, node);
                else
                    directionalNeighbours.Add(direction.top, node);
            }

            //check if the neighbour is to the right
            if(node.pos.x > pos.x && node.pos.y == pos.y)
            {
                directionalNeighbours.Add(direction.right, node);
            }

            //check if the neighbour is below (bottomLeft, bottom, bottomRight)
            if(node.pos.y < pos.y)
            {
                if(node.pos.x > pos.x)
                    directionalNeighbours.Add(direction.bottomRight, node);
                else if(node.pos.x < pos.x)
                    directionalNeighbours.Add(direction.bottomLeft, node);
                else
                    directionalNeighbours.Add(direction.bottom, node);
            }

            //check if the neighbour is to the left
            if(node.pos.x < pos.x && node.pos.y == pos.y)
                directionalNeighbours.Add(direction.left, node);
        }
    }

    public Node CheckNeighbour(direction dir, int playerID)
    {
        if (!directionalNeighbours.ContainsKey(dir)) return null;
        
        //if the neighbour contains a coin belonging to the correct player, return the neighbour
        if (directionalNeighbours[dir].ownerID == playerID && directionalNeighbours[dir].occupied)
        {
            return directionalNeighbours[dir];
        }
        else
        {
            return null;
        }
    }

    public void SetMultiplier(float multiplier)
    {
        this.multiplier = multiplier;
        multiplierText.enabled = true;
        multiplierText.text = $"{multiplier}x";
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
        multiplierText.enabled = false;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;

        if(value)
            multiplierText.color = Color.black;
        else
            multiplierText.color = Color.white;
    }

    public void PlayParticles()
    {
        particles.Play();
    }

    public void StopParticles()
    {
        particles.Stop();
    }
}
