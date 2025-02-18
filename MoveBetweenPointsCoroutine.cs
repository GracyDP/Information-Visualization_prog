using UnityEngine;
using System.Collections;

public class MoveBetweenPointsCoroutine : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float duration = 2.0f;

    void Start()
    {
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToPosition(pointB));
            yield return StartCoroutine(MoveToPosition(pointA));
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}