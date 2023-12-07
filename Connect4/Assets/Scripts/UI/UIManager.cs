using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [Header("General UI")]
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject MainMenuUI;

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
    [SerializeField] private float bonusScoreTransitionSpeed;
    private string bonusScoreText;

    [Header("Health UI")]
    [SerializeField] private Slider p1Health;
    [SerializeField] private Slider p2Health;

    [Header("External Component")]
    [SerializeField] private GameManagerScript gameManager;

    public bool canAnimateScore;

    private void Start()
    {
        GameManagerScript.gameStarted.AddListener(SwapHUDs);
    }

    //Swap the turn timer UI to the other player color
    public void SwapTurnUI()
    {
        if(TurnManager.playerID == TurnManager.player1)
            coinImage.color = player1Color;
        else
            coinImage.color = player2Color;
    }
    
    //Set the correct target for the "losing health" animation
    public void AdjustPlayerHealth(int playerID, float adjustment)
    {
        if(playerID == TurnManager.player1)
            StartCoroutine(AnimateHealthLost(p1Health, adjustment));
        else
            StartCoroutine(AnimateHealthLost(p2Health, adjustment));
    }

    //Animate healthbar "smoothly" losing health
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

        gameManager.ResetBoard(); // =========================== CONSIDER FIXING THIS ==========================================
    }

    public void DisplayWinText(int playerID)
    {
        winText.text = $"Player {playerID + 1} wins!";
        winText.gameObject.SetActive(true);
    }

    //Swap between main menu and game HUD
    public void SwapHUDs()
    {
        if(GameUI.activeSelf)
        {
            MainMenuUI.SetActive(true);
            GameUI.SetActive(false);
        }
        else
        {
            MainMenuUI.SetActive(false);
            GameUI.SetActive(true);
        }
    }

    // ======================================== ANIMATE SCORE UI ===================================================

    public void DisplayScoreText(int playerID, int score, int bonusScore, float multiplier)
    {
        //set the base score UI
        totalScoreUI.score = score;
        totalScoreUI.SetText(score.ToString());

        StartCoroutine(ScoreTallyCo(playerID, bonusScore, multiplier));
    }

    // add the bonus score and multiplier if applicable and then apply the total score to opponent health
    private IEnumerator ScoreTallyCo(int playerID, int bonusScore, float multiplier)
    {
        //apply bonus score
        if (bonusScore > 0)
        {
            canAnimateScore = false;
            AddBonusScore(bonusScore);

            //wait for animation to end
            while (!canAnimateScore)
                yield return null;
        }
        //apply multiplier
        if (multiplier != 1)
        {
            canAnimateScore = false;
            ApplyMultiplier(multiplier);

            //wait for animation to end
            while (!canAnimateScore)
                yield return null;
        }

        //play the "apply score" UI animation for the correct player
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
        yield return new WaitForSeconds(bonusScoreTransitionSpeed);
        bonusScoreUI.SetText(bonusScoreText);
        yield return new WaitForSeconds(bonusScoreTransitionSpeed);
        bonusScoreUI.PlayAnimation(BonusScoreUI.addScoreAnim);
    }
}
