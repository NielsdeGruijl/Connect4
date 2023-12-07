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
        float healthAdjustment;

        //convert score to actual damage
        int damage = Mathf.RoundToInt(score / scoreToDamageConversion);

        //convert damage to a percentage of the max health to adjust the healthbar UI
        if (playerID == TurnManager.player1)
            healthAdjustment = (float)damage / p2MaxHealth;
        else
            healthAdjustment = (float)damage / p1MaxHealth;

        UIManager.AdjustPlayerHealth(playerID, healthAdjustment);
    }
}
