using UnityEngine;

public class RotateSky : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
