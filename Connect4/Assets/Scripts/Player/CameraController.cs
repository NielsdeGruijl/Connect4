using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCam;

    [SerializeField] private AnimationCurve curve;

    private void Start()
    {
        GameManagerScript.gameStarted.AddListener(RotateCamera);
    }

    private void RotateCamera()
    { 
        StartCoroutine(RotateCameraCo());
    }

    //Rotates the camera 180 degrees over time to switch between the main menu and game
    private IEnumerator RotateCameraCo()
    {
        CinemachinePOV pov = vCam.GetCinemachineComponent<CinemachinePOV>();
        float currentRotation = pov.m_HorizontalAxis.Value;
        float targetRotation = 180;

        float timeElapsed = 0;

        while (curve[curve.length-1].time > timeElapsed)
        {
            timeElapsed += Time.deltaTime;

            pov.m_HorizontalAxis.Value = currentRotation + curve.Evaluate(timeElapsed) * targetRotation;

            yield return null;
        }
    }
}
