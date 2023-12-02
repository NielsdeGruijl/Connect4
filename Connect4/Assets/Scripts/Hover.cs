using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float magnitude;
    [SerializeField] private float frequency;

    private void Update()
    {
        //I had to animate the board this way because there is a bug in this Unity version where you can't properly edit animation curves so the animation was stuttering
        Vector3 position = new Vector3();
        position.y = magnitude * Mathf.Cos(frequency * Time.time);
        transform.position = position;
    }
}
