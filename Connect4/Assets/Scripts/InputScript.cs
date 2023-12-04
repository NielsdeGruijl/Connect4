using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    private CoinPlacer coinPlacer;
    private TurnManager turnManager;

    public bool inputLocked = false;

    private void Start()
    {
        coinPlacer = GetComponent<CoinPlacer>();
        turnManager = GetComponent<TurnManager>();
    }

    void Update()
    {
        if (!inputLocked)
        {
            MoveCoinInput();
            CameraInput();
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

    void CameraInput()
    {
        int inputDir = 0;
        if(Input.GetKey(KeyCode.A))
            inputDir = 1;
        if (Input.GetKey(KeyCode.D))
            inputDir = -1;

        if (inputDir != 0)
            cameraController.OrbitBoard(inputDir);
    }
}
