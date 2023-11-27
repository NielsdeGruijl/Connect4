using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    [SerializeField] private CoinPlacer coinPlacer;
    [SerializeField] private CameraController cameraController;

    private bool inputLocked = false;

    void Update()
    {
        if (!inputLocked)
        {
            PlayerInput();
            CameraInput();
        }
    }

    public void LockInput(bool value)
    {
        inputLocked = value;
    }

    void PlayerInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            coinPlacer.SelectPosition();
        }
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
