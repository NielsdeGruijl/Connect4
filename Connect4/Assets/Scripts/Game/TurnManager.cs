using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float turnDuration;

    [Header("External Components")]
    [SerializeField] private UIManager uiManager;

    private CoinPlacer coinPlacer;
    private InputScript input;

    //setting the playerIDs all other scripts will reference
    public const int player1 = 0;
    public const int player2 = 1;
    [HideInInspector] public static int playerID { get; private set; }

    private Coroutine timerCoroutine;

    private void Start()
    {
        input = GetComponent<InputScript>();
        coinPlacer = GetComponent<CoinPlacer>();
        playerID = player1;
        timerCoroutine = StartCoroutine(TurnTimerCo());
        coinPlacer.SpawnCoin(playerID);

        uiManager.UpdateTurnUI(turnDuration);
    }

    private IEnumerator TurnTimerCo()
    {
        yield return new WaitForSeconds(turnDuration);
        ChangeTurns(0.1f);
    }

    public void ChangeTurns(float duration)
    {
        StartCoroutine(ChangeTurnsCo(duration));
    }

    private IEnumerator ChangeTurnsCo(float duration)
    {
        input.inputLocked = true;

        yield return new WaitForSeconds(duration);
        
        if (playerID == player1)
            playerID = player2;
        else
            playerID = player1;

        coinPlacer.DestroyCoin();
        coinPlacer.SpawnCoin(playerID);

        input.inputLocked = false;

        uiManager.UpdateTurnUI(turnDuration);
        
        StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TurnTimerCo());
    }
}
