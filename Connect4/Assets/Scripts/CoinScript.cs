using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    public Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 distanceToTarget;

    public float animationLength { get { return curve.keys[1].time; } }

    private void Start()
    {
        startPos = transform.position;
        distanceToTarget = targetPos - startPos;

        StartCoroutine(AnimateCoinFallCo());
    }

    private IEnumerator AnimateCoinFallCo()
    {
        float timeElapsed = 0;
        while (timeElapsed < curve.length)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
            transform.position = startPos + distanceToTarget * curve.Evaluate(timeElapsed);
        }
        transform.position = startPos + distanceToTarget * curve.Evaluate(curve.keys[1].time);
    }
}
