using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float camOrbitSpeed;

    public void OrbitBoard(int dir)
    { 
        transform.Rotate(new Vector3(0, camOrbitSpeed * dir * Time.deltaTime, 0));
    }
}
