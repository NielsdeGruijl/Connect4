using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    private Vector3 startPos;
    private Vector3 distanceToTarget;

    public float animationLength { get { return curve.keys[curve.length - 1].time; } }

    //Play the falling animation
    public void DropCoin(Vector3 targetPos)
    {
        startPos = transform.localPosition;
        distanceToTarget = targetPos - startPos;
        StartCoroutine(AnimateCoinFallCo(targetPos));
    }

    private IEnumerator AnimateCoinFallCo(Vector3 targetPos)
    {
        float timeElapsed = 0;
        while (timeElapsed < animationLength)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
            transform.localPosition = startPos + distanceToTarget * curve.Evaluate(timeElapsed);
        }
        transform.localPosition = targetPos;
    }
}
