using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("External Components")]
    [SerializeField] private GameManagerScript gameManager;

    public bool canAnimateScore;

    private void Start()
    {
        gameManager.gameStarted.AddListener(SwapHUDs);
        gameManager.gameStarted.AddListener(ResetHealthUI);
        gameManager.gameEnded.AddListener(SwapHUDs);
        gameManager.gameEnded.AddListener(DisableScoreUI);
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
    public void AdjustPlayerHealth(int playerID, float currentHealth, float targetHealth)
    {
        if(playerID == TurnManager.player1)
            StartCoroutine(AnimateHealthLost(p1Health, currentHealth, targetHealth));
        else
            StartCoroutine(AnimateHealthLost(p2Health, currentHealth, targetHealth));
    }

    //Animate healthbar "smoothly" losing health
    private IEnumerator AnimateHealthLost(Slider playerHealth, float currentHealth, float targetHealth)
    {
        while (Mathf.Abs(currentHealth - targetHealth) >= 0.01)
        {
            yield return null;
            currentHealth = Mathf.Lerp(currentHealth, targetHealth, 0.01f);
            playerHealth.value = Mathf.Clamp(currentHealth, 0, 1);
        }
        playerHealth.value = Mathf.Clamp(targetHealth, 0, 1);

        yield return new WaitForSeconds(0.1f);

        //either start a new round or end the match
        if (targetHealth > 0)
            gameManager.ResetBoard();
        else
            StartCoroutine(DisplayWinUI());
    }

    private void ResetHealthUI()
    {
        p1Health.value = 1;
        p2Health.value = 1;
    }

    public IEnumerator DisplayWinUI()
    {
        winText.text = $"Player {TurnManager.playerID + 1} wins!";
        winText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        winText.gameObject.SetActive(false);
        gameManager.EndGame();
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

    private void DisableScoreUI()
    {
        totalScoreUI.SetActive(0);
        bonusScoreUI.SetActive(0);
    }
}
