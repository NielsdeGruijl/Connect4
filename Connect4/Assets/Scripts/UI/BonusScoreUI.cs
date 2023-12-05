using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonusScoreUI : MonoBehaviour
{
    public const string idleAnim = "Idle";
    public const string activeIdleAnim = "Idle";
    public const string addScoreAnim = "AddScore";

    [SerializeField] private Animator animator;
    [SerializeField] private TotalScoreUI totalScoreUI;
    private TMP_Text scoreText;
    private string currentAnim;

    public int score;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
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

    public void ResetPosition()
    {
        Debug.Log("Bonus: " + startPos);
        transform.localPosition = startPos;
    }
    
    //Apply score to total score, called at the end of AddScore animation
    public void AddScore()
    {
        totalScoreUI.AddScore(score);
    }
    
    public void SetActive(int value)
    {
        bool enabled;
        if (value == 0)
            enabled = false;
        else
            enabled = true;

        scoreText.enabled = enabled;
    }
}
