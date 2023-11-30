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

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image coinImage;
    [SerializeField] private Color redColor;
    [SerializeField] private Color greenColor;
   

    private InputScript input;

    //setting the playerIDs all other scripts will reference
    public const int player1 = 0;
    public const int player2 = 1;
    [Range(player1, player2)] private int playerID;
    public int getPlayerID { get { return playerID; } }

    private Coroutine timerCoroutine;

    private void Start()
    {
        input = GetComponent<InputScript>();
        playerID = player1;
        timerCoroutine = StartCoroutine(TurnTimerCo());
    }

    private IEnumerator TurnTimerCo()
    {
        float timeElapsed = turnDuration;
        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeElapsed).ToString();
            yield return null;
        }
        ChangeTurns(0.1f);
    }

    public void ChangeTurns(float duration)
    {
        StartCoroutine(ChangeTurnsCo(duration));
    }

    private IEnumerator ChangeTurnsCo(float duration)
    {
        input.InputLocked = true;

        yield return new WaitForSeconds(duration);
        
        if (playerID == player1)
        {
            playerID = player2;
            coinImage.color = greenColor;
        }
        else
        {
            playerID = player1;
            coinImage.color = redColor;
        }

        input.InputLocked = false;
        
        StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TurnTimerCo());
    }
}
