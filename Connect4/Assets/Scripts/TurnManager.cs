using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private float turnDuration;
    [HideInInspector] public bool player1Turn = true;
    [HideInInspector] public bool player2Turn = false;

    private void Start()
    {
        StartCoroutine(TurnTimerCo());
    }

    private IEnumerator TurnTimerCo()
    {
        yield return new WaitForSeconds(turnDuration);
        ChangeTurns();
    }

    public void ChangeTurns()
    {
        player2Turn = player1Turn;
        player1Turn = !player2Turn;
        //Debug.Log("Changed Turns!");
        StopAllCoroutines();
        StartCoroutine(TurnTimerCo());
    }
}
