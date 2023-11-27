using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private float turnDuration;

    public const int player1 = 0;
    public const int player2 = 1;
    [Range(player1, player2)] private int playerID;
    public int getPlayerID { get { return playerID; } }

    private void Start()
    {
        playerID = player1;
        StartCoroutine(TurnTimerCo());
    }

    private IEnumerator TurnTimerCo()
    {
        yield return new WaitForSeconds(turnDuration);
        ChangeTurns();
    }

    public void ChangeTurns()
    {
        if(playerID == player1)
            playerID = player2;
        else
            playerID = player1;

        StopAllCoroutines();
        StartCoroutine(TurnTimerCo());
    }
}
