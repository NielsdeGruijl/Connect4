using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float camOrbitSpeed;

    private void FixedUpdate()
    {
        OrbitBoard();
    }

    void OrbitBoard()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, camOrbitSpeed, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, -camOrbitSpeed, 0));
        }
    }
}
