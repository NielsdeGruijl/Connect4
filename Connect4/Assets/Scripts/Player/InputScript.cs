using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
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
        if (Input.GetKeyDown(KeyCode.A))
            coinPlacer.MoveCoinToColumn(-1);
        if (Input.GetKeyDown(KeyCode.D))
            coinPlacer.MoveCoinToColumn(1);
        if (Input.GetKeyDown(KeyCode.S))
            coinPlacer.LockInColumn();

/*        if (turnManager.PlayerID == TurnManager.player1)
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                coinPlacer.MoveCoinToColumn(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                coinPlacer.MoveCoinToColumn(1);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                coinPlacer.LockInColumn();
        }*/
    }
}
