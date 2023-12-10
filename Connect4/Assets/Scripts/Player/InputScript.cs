using UnityEngine;

public class InputScript : MonoBehaviour
{
    private CoinPlacer coinPlacer;

    public bool inputLocked = false;

    private void Start()
    {
        coinPlacer = GetComponent<CoinPlacer>();
    }

    void Update()
    {
        if (!inputLocked)
        {
            MoveCoinInput();
        }
    }

    void MoveCoinInput()
    {
        if (TurnManager.playerID == TurnManager.player1)
        {
            if (Input.GetKeyDown(KeyCode.A))
                coinPlacer.MoveCoinToColumn(-1);
            if (Input.GetKeyDown(KeyCode.D))
                coinPlacer.MoveCoinToColumn(1);
            if (Input.GetKeyDown(KeyCode.S))
                coinPlacer.LockInColumn();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                coinPlacer.MoveCoinToColumn(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                coinPlacer.MoveCoinToColumn(1);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                coinPlacer.LockInColumn();
        }
    }
}
