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
/*    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bonusScoreText;*/

    [Header("Health UI")]
    [SerializeField] private GameObject p1HealthGO;
    [SerializeField] private GameObject p2HealthGO;
    [SerializeField] private Slider p1Health;
    [SerializeField] private Slider p2Health;

    [Header("External Component")]
    [SerializeField] private TurnManager turnManager;
    //[SerializeField] private HealthManager playerHealth;
    [SerializeField] private GameManagerScript gameManager;

    private Coroutine turnTimerCoroutine;

    private Vector3 bonusScoreTextPosition;

    private int score;
    public bool canAnimateScore;

    public void UpdateTurnUI(float turnDuration)
    {
        if(turnManager.PlayerID == TurnManager.player1)
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
            Debug.Log(health);
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

    public void ApplyMultiplier(float multiplier)
    {
        int scoreToAdd = Mathf.CeilToInt(score * multiplier - 1);
        StartCoroutine(SetBonusScoreCo("Mutliplier", scoreToAdd));
    }

    //Display the bonus score and its source
    private IEnumerator SetBonusScoreCo(string source, int bonusScore)
    {
        bonusScoreUI.score = bonusScore;
        bonusScoreUI.SetText($"{source}:");
        yield return new WaitForSeconds(1);
        bonusScoreUI.SetText($"{bonusScore}");
        yield return new WaitForSeconds(1);
        bonusScoreUI.PlayAnimation(BonusScoreUI.addScoreAnim);
    }

    // add the bonus score and multiplier if applicable and then apply the total score to opponent health
    private IEnumerator ScoreTallyCo(int playerID, int bonusScore, float multiplier)
    {
        if (bonusScore > 0)
        {
            canAnimateScore = false;
            StartCoroutine(SetBonusScoreCo("Bonus coins", bonusScore));

            while (!canAnimateScore)
            {
                yield return null;
            }
        }
        if(multiplier != 1)
        {
            
            canAnimateScore = false;
            ApplyMultiplier(multiplier);

            while(!canAnimateScore)
            {
                Debug.Log(canAnimateScore);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

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



/*    private IEnumerator AddScoreCo(int scoreToAdd)
    {
        int targetScore = score + scoreToAdd;
        float _currentScore = score;

        yield return new WaitForSeconds(0.2f);

        while (score < targetScore)
        {
            yield return null;
            _currentScore = Mathf.Lerp(_currentScore, targetScore, 0.01f);
            score = Mathf.CeilToInt(_currentScore);
            scoreText.text = this.score.ToString();
        }
        canAnimateScore = true;
    }*/

    /*    private IEnumerator AnimateBonusScoreCo(int bonusScore)
        {
            Vector3 currentpos = bonusScoreText.rectTransform.localPosition;
            Vector3 targetpos = scoreText.rectTransform.localPosition;
            Color targetColor = new Color(bonusScoreText.color.r, bonusScoreText.color.g, bonusScoreText.color.b, 0);

            while ((targetpos - currentpos).magnitude > 0.1f)
            {
                yield return null;
                currentpos = Vector3.Lerp(currentpos, targetpos, 0.1f);
                bonusScoreText.rectTransform.localPosition = currentpos;
                bonusScoreText.color = Color.Lerp(bonusScoreText.color, targetColor, 0.1f);
            }

            bonusScoreText.gameObject.SetActive(false);
            bonusScoreText.rectTransform.localPosition = Vector3.zero;
            StartCoroutine(AddScoreCo(bonusScore));
        }*/

    /*private IEnumerator ApplyScoreAnim(int playerID)
    {
        Vector3 healthPos;
        if (playerID == TurnManager.player1)
            healthPos = p2HealthGO.transform.localPosition;
        else
            healthPos = p1HealthGO.transform.localPosition;

        Vector3 startPos = scoreText.rectTransform.localPosition;
        Vector3 scorePos = startPos;

        yield return new WaitForSeconds(0.2f);

        while ((healthPos - scorePos).magnitude > 10)
        {
            scorePos = Vector3.Lerp(scorePos, healthPos, 0.01f);
            scoreText.rectTransform.localPosition = scorePos;
            yield return null;
        }

        scoreText.gameObject.SetActive(false);
        scoreText.rectTransform.localPosition = startPos;
        playerHealth.ApplyDamage(playerID, score);

        yield return new WaitForSeconds(1f);

        gameManager.ResetBoard();
    }*/
}
