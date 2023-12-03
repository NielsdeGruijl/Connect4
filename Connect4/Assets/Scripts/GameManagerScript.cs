using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] private CoinPlacer coinPlacer;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = GetComponent<UIManager>();
    }

    public void HandleConnect4(int playerID, List<Node> connectedNodes)
    {
        score = baseConnect4Score;
        int bonusScore = bonusCoinScore * (connectedNodes.Count - 4);
        float multiplier = GetMultiplier(connectedNodes);

        uiManager.DisplayScoreText(playerID, score, bonusScore, multiplier);
    }

    private float GetMultiplier(List<Node> nodes)
    {
        float tempMultiplier = 1;
        foreach(Node node in nodes)
        {
            //Debug.Log("Multiplier: " + node.multiplier);
            if (node.multiplier <= 0)
                tempMultiplier = 0;
            else
                tempMultiplier *= node.multiplier;
        }

        return tempMultiplier;
    }

    public void ResetBoard()
    {
        grid.ClearGrid();
        turnManager.ChangeTurns(0.1f);
        input.inputLocked = false;
        coinPlacer.gameWon = false;
    }
}
