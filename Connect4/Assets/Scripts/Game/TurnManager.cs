using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float turnDuration;

    [Header("External Components")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CoinPlacer coinPlacer;
    [SerializeField] private InputScript input;
    [SerializeField] private GameManagerScript gameManager;

    //setting the playerIDs all other scripts will reference
    public const int player1 = 0;
    public const int player2 = 1;
    [HideInInspector] public static int playerID { get; private set; }

    private float turnTimeElapsed;
    private bool paused = true;

    private void Start()
    {
        gameManager.gameStarted.AddListener(StartGame);
        gameManager.gameEnded.AddListener(EndGame);
    }

    private void Update()
    {
        if(paused) return;

        //turn timer
        if (turnTimeElapsed > 0)
        {
            turnTimeElapsed -= Time.deltaTime;
            uiManager.timerText.text = Mathf.CeilToInt(turnTimeElapsed).ToString();
        }
        else
            ChangeTurns();
    }

    private void StartGame()
    {
        playerID = player1;
        turnTimeElapsed = turnDuration;
        paused = false;
        coinPlacer.SpawnCoin(playerID);

        if(uiManager)
            uiManager.SwapTurnUI();
    }

    private void EndGame()
    {
        paused = true;
        StopAllCoroutines();
    }

    public void ChangeTurns(float duration = 0.1f)
    {
        StartCoroutine(ChangeTurnsCo(duration));
    }

    private IEnumerator ChangeTurnsCo(float duration)
    {
        paused = true;
        input.inputLocked = true;

        yield return new WaitForSeconds(duration);
        
        //swap player IDs
        if (playerID == player1)
            playerID = player2;
        else
            playerID = player1;

        //swap coin prefab
        coinPlacer.DestroyCoin();
        coinPlacer.SpawnCoin(playerID);

        paused = false;
        input.inputLocked = false;

        if(uiManager)
            uiManager.SwapTurnUI();

        //reset the turn duration timer
        paused = false;
        turnTimeElapsed = turnDuration;
    }

    public void Pause()
    {
        StopAllCoroutines();
        paused = true;
    }
}
