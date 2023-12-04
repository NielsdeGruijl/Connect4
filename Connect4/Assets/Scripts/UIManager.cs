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
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bonusScoreText;

    [Header("Health UI")]
    [SerializeField] private GameObject p1HealthGO;
    [SerializeField] private GameObject p2HealthGO;
    [SerializeField] private Slider p1Health;
    [SerializeField] private Slider p2Health;

    [Header("External Component")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private HealthManager playerHealth;
    [SerializeField] private GameManagerScript gameManager;

    private Coroutine turnTimerCoroutine;

    private Vector3 bonusScoreTextPosition;

    private int score;
    private bool canAnimateScore;

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
            StartCoroutine(AnimateHealthLost(p2Health, adjustment));
        else
            StartCoroutine(AnimateHealthLost(p1Health, adjustment));
    }

    private IEnumerator AnimateHealthLost(Slider playerHealth, float adjustment)
    {
        float health = playerHealth.value;
        float newHealth = health - adjustment;

        while (health > newHealth)
        {
            health = Mathf.Lerp(health, newHealth, 0.1f);
            playerHealth.value = Mathf.Clamp(health, 0, 1);
            yield return null;
        }
        playerHealth.value = Mathf.Clamp(newHealth, 0, 1);
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
        scoreText.text = score.ToString();
        scoreText.gameObject.SetActive(true);

        StartCoroutine(ScoreTallyCo(playerID, bonusScore, multiplier));
    }

    public void AddScore(int scoreToAdd)
    {
        StartCoroutine(DisplayBonusScoreCo("Bonus coins", scoreToAdd));
    }

    public void ApplyMultiplier(float multiplier)
    {
        int scoreToAdd = Mathf.CeilToInt(score * multiplier - 1);
        StartCoroutine(DisplayBonusScoreCo("Mutliplier", scoreToAdd));
    }

    //Display the bonus score and its source

    private IEnumerator DisplayBonusScoreCo(string source, int bonusScore)
    {
        bonusScoreText.gameObject.SetActive(true);
        bonusScoreText.text = $"{source}:";
        yield return new WaitForSeconds(1);
        bonusScoreText.text = $"{bonusScore}";
        yield return new WaitForSeconds(1);
        StartCoroutine(AnimateBonusScoreCo(bonusScore));
    }

    // fade the bonus score text out while moving it towards the actual score text

    private IEnumerator AnimateBonusScoreCo(int bonusScore)
    {
        Vector3 currentpos = bonusScoreText.rectTransform.localPosition;
        Vector3 targetpos = scoreText.rectTransform.localPosition;
        Color targetColor = new Color(bonusScoreText.color.r, bonusScoreText.color.g, bonusScoreText.color.b, 0);

        while((targetpos - currentpos).magnitude > 0.1f)
        {
            yield return null;
            currentpos = Vector3.Lerp(currentpos, targetpos, 0.1f);
            bonusScoreText.rectTransform.localPosition = currentpos;
            bonusScoreText.color = Color.Lerp(bonusScoreText.color, targetColor, 0.1f);
        }

        bonusScoreText.gameObject.SetActive(false);
        bonusScoreText.rectTransform.localPosition = Vector3.zero;
        StartCoroutine(AddScoreCo(bonusScore));
    }

    // add bonus score to the actual score over time

    private IEnumerator AddScoreCo(int scoreToAdd)
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
    }

    private IEnumerator ApplyScoreAnim(int playerID)
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
    }

    // add the bonus score and multiplier if applicable

    private IEnumerator ScoreTallyCo(int playerID, int bonusScore, float multiplier)
    {
        if (bonusScore > 0)
        {
            canAnimateScore = false;
            AddScore(bonusScore);

            while(!canAnimateScore)
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

        StartCoroutine(ApplyScoreAnim(playerID));
            
        //scoreText.GetComponent<Animator>().SetTrigger("P1");
    }
}
