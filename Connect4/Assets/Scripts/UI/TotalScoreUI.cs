using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TotalScoreUI : MonoBehaviour
{
    //Animations
    public const string idleAnim = "Idle";
    public const string activeIdleAnim = "ActiveIdle";
    public const string applyP1Anim = "ApplyP1";
    public const string applyP2Anim = "ApplyP2";

    [SerializeField] private Animator animator;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private UIManager uiManager;
    private TMP_Text scoreText;
    
    private string currentAnim;

    public int score;

    public int targetID;

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.enabled = false;
    }

    public void PlayAnimation(string anim)
    {
        if (currentAnim == anim) return;
        animator.Play(anim);
        currentAnim = anim;
    }

    public void SetText(string text)
    {
        scoreText.enabled = true;
        scoreText.text = text;
    }

    //Apply the score to opponent health, called at the end of ApplyP1/P2 animation
    public void ApplyScore()
    {
        healthManager.ApplyDamage(targetID, score);
    }

    public void SetActive(int value)
    {
        bool enabled;
        if(value == 0)
            enabled = false;
        else
            enabled = true;
            

        scoreText.enabled = enabled;
    }
    
    //Adds potential bonus score to the total score
    public void AddScore(int score)
    {
        StartCoroutine(AddScoreCo(score));
    }

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
        uiManager.canAnimateScore = true;
    }
}
