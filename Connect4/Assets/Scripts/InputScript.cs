using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    [SerializeField] private CoinPlacer coinPlacer;
    [SerializeField] private CameraController cameraController;

    private bool inputLocked = false;
    public bool InputLocked { set { inputLocked = value; } get { return inputLocked; } }

    void Update()
    {
        if (!inputLocked)
        {
            PlayerInput();
            CameraInput();
        }
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
