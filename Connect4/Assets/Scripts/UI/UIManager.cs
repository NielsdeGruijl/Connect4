using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [Header("Turn UI")]
    [SerializeField] private Color player1Color;
    [SerializeField] private Color player2Color;
    [SerializeField] private Image coinImage;
    public TMP_Text timerText;

    [Header("Win UI")]
    [SerializeField] private TMP_Text winText;

    [Header("Score UI")]
    [SerializeField] private TotalScoreUI totalScoreUI;
    [SerializeField] private BonusScoreUI bonusScoreUI;
    private string bonusScoreText;

    [Header("Health UI")]
    [SerializeField] private Slider p1Health;
    [SerializeField] private Slider p2Health;

    [Header("External Component")]
    [SerializeField] private GameManagerScript gameManager;

    private Coroutine turnTimerCoroutine;

    private int score;
    public bool canAnimateScore;

    public void UpdateTurnUI(float turnDuration)
    {
        if(TurnManager.playerID == TurnManager.player1)
            coinImage.color = player1Color;
        else
            coinImage.color = player2Color;

        if(turnTimerCoroutine != null)
            StopCoroutine(turnTimerCoroutine);
        turnTimerCoroutine = StartCoroutine(TurnTimerCo(turnDuration));
    }

    private IEnumerator TurnTimerCo(float turnDuration)
    {
        float timeElapsed = turnDuration;
        while (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeElapsed).ToString();
            yield return null;
        }
    }

    public void AdjustPlayerHealth(int playerID, float adjustment)
    {
        if(playerID == TurnManager.player1)
            StartCoroutine(AnimateHealthLost(p1Health, adjustment));
        else
            StartCoroutine(AnimateHealthLost(p2Health, adjustment));
    }

    private IEnumerator AnimateHealthLost(Slider playerHealth, float adjustment)
    {
        float health = playerHealth.value;
        float newHealth = health - adjustment;

        while (health - newHealth >= 0.01)
        {
            yield return null;
            health = Mathf.Lerp(health, newHealth, 0.1f);
            playerHealth.value = Mathf.Clamp(health, 0, 1);
        }
        playerHealth.value = Mathf.Clamp(newHealth, 0, 1);
        gameManager.ResetBoard();
    }

    public void DisplayWinText(int playerID)
    {
        winText.text = $"Player {playerID + 1} wins!";
        winText.gameObject.SetActive(true);
    }


    // ======================================== ANIMATE SCORE UI ===================================================

    public void DisplayScoreText(int playerID, int score, int bonusScore, float multiplier)
    {
        StopCoroutine(turnTimerCoroutine);
        this.score = score;
        totalScoreUI.score = score;
        totalScoreUI.SetText(score.ToString());

        StartCoroutine(ScoreTallyCo(playerID, bonusScore, multiplier));
    }

    public void AddBonusScore(int bonusScore)
    {
        bonusScoreUI.score = (int)bonusScore;
        bonusScoreText = bonusScore.ToString();
        StartCoroutine(DisplayBonusScoreCo("Bonus coins"));
    }

    public void ApplyMultiplier(float multiplier)
    {
        bonusScoreText = $"{multiplier}x";
        multiplier -= 1;
        bonusScoreUI.score = Mathf.CeilToInt(totalScoreUI.score * multiplier);
        StartCoroutine(DisplayBonusScoreCo("Mutliplier"));
    }

    //Display the bonus score and its source
    private IEnumerator DisplayBonusScoreCo(string source)
    {
        bonusScoreUI.SetText($"{source}:");
        yield return new WaitForSeconds(1);
        bonusScoreUI.SetText(bonusScoreText);
        yield return new WaitForSeconds(1);
        bonusScoreUI.PlayAnimation(BonusScoreUI.addScoreAnim);
    }

    // add the bonus score and multiplier if applicable and then apply the total score to opponent health
    private IEnumerator ScoreTallyCo(int playerID, int bonusScore, float multiplier)
    {
        if (bonusScore > 0)
        {
            canAnimateScore = false;
            AddBonusScore(bonusScore);

            //wait for animation to end
            while (!canAnimateScore)
                yield return null;
        }
        if(multiplier != 1)
        {
            
            canAnimateScore = false;
            ApplyMultiplier(multiplier);

            //wait for animation to end
            while (!canAnimateScore)
                yield return null;
        }

        if (playerID == TurnManager.player1)
        {
            totalScoreUI.PlayAnimation(TotalScoreUI.applyP2Anim);
            totalScoreUI.targetID = TurnManager.player2;
        }
        else
        {
            totalScoreUI.PlayAnimation(TotalScoreUI.applyP1Anim);
            totalScoreUI.targetID = TurnManager.player1;
        }
    }
}
