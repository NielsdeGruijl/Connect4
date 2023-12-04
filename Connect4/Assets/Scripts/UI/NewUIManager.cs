using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bonusScoreText;

    public void DisplayScoreText(int playerID, int score, int bonusScore, float multiplier)
    {

    }
}
