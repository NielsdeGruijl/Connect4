using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManagerScript : MonoBehaviour
{
    [Header("Score values")]
    [SerializeField] private int baseConnect4Score;
    [SerializeField] private int bonusCoinScore;
    private int score;

    [Header("External components")]
    [SerializeField] private GridScript grid;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private InputScript input;
    private UIManager uiManager;

    public static UnityEvent gameStarted;
    public static UnityEvent gameEnded;

    private void Awake()
    {
        gameStarted ??= new UnityEvent();
        gameEnded ??= new UnityEvent();

        uiManager = GetComponent<UIManager>();
    }

    public void CheckConnect4(Node node, int playerID, float coinFallDuration)
    {
        List<Node> connectedNodes = new List<Node>();

        //Check for 4 connected coins along each possible axis (grid.FindConnect4 checks both the given and opposite direction)
        for (int i = 0; i < 4; i++)
        {
            //get the direction enum by integer and check that axis
            Node.direction dir = (Node.direction)i;
            connectedNodes = grid.FindConnect4(node, dir, playerID);

            //if a connect 4 was found, handle it
            if (connectedNodes.Count >= 4)
            {
                HandleConnect4(playerID, connectedNodes);
                break;
            }
        }

        //no connect 4 found, change turns
        turnManager.ChangeTurns(coinFallDuration);
    }

    public void HandleConnect4(int playerID, List<Node> connectedNodes)
    {
        //get score values of the connect 4
        score = baseConnect4Score;
        int bonusScore = bonusCoinScore * (connectedNodes.Count - 4);
        float multiplier = GetMultiplier(connectedNodes);

        //set the UI animations in motion and stop the turn manager
        uiManager.DisplayScoreText(playerID, score, bonusScore, multiplier);
        turnManager.StopAllCoroutines();
    }

    //Calculates the total multiplier of the connect 4
    private float GetMultiplier(List<Node> nodes)
    {
        float tempMultiplier = 1;
        foreach(Node node in nodes)
        {
            if (node.multiplier <= 0)
                tempMultiplier = 0;
            else
                tempMultiplier *= node.multiplier;
        }

        return tempMultiplier;
    }

    //starts the game, called on the "Play" button in the main menu
    public void StartGame()
    {
        gameStarted?.Invoke();
    }

    public void ResetBoard()
    {
        grid.ClearGrid();
        turnManager.ChangeTurns();
        input.inputLocked = false;
    }
}
