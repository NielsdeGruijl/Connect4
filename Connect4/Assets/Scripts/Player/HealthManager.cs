using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int p1MaxHealth;
    [SerializeField] private int p2MaxHealth;
    [SerializeField] private int scoreToDamageConversion;

    [Header("External components")]
    [SerializeField] private UIManager UIManager;
    [SerializeField] private GameManagerScript gameManager;

    private float p1Health;
    private float p2Health;

    private void Start()
    {
        gameManager.gameStarted.AddListener(SetHealth);
    }

    private void SetHealth()
    {
        p1Health = p1MaxHealth;
        p2Health = p2MaxHealth;
    }

    public void ApplyDamage(int targetID, int score)
    {
        float healthAdjustment;

        //convert score to actual damage
        int damage = Mathf.RoundToInt(score / scoreToDamageConversion);

        //apply damage to health and convert it to a percentage of the max health to adjust the healthbar UI
        if (targetID == TurnManager.player1)
        {
            healthAdjustment = (float)damage / p1MaxHealth;
            p1Health -= damage;
        }
        else
        {
            healthAdjustment = (float)damage / p2MaxHealth;
            p2Health -= damage;
        }

        UIManager.AdjustPlayerHealth(targetID, healthAdjustment);
    }
}
