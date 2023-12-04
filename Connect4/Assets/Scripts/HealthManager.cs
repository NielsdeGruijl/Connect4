using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int p1MaxHealth;
    [SerializeField] private int p2MaxHealth;
    [SerializeField] private int scoreToDamageConversion;

    [Header("External components")]
    [SerializeField] private UIManager UIManager;

    public void ApplyDamage(int playerID, int score)
    {
        //Debug.Log(playerID);
        float healthAdjustment = 0;

        int tempScore = Mathf.RoundToInt(score / scoreToDamageConversion);

        if (playerID == TurnManager.player1)
            healthAdjustment = (float)tempScore / p2MaxHealth;
        else
            healthAdjustment = (float)tempScore / p1MaxHealth;

        UIManager.AdjustPlayerHealth(playerID, healthAdjustment);
    }
}
