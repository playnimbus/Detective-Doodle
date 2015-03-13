using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{   
    [Range(0, 1)]
    public float lerpSpeed;

    private Transform target;
    private Vector3 offset;
    private Quaternion initialRotation;

    private Coroutine followCoroutine;
    private Coroutine vantageCoroutine;

    public void Init(Transform target)
    {
        this.target = target;
        offset = transform.position - target.position;
        initialRotation = transform.rotation;
        followCoroutine = StartCoroutine(FollowCoroutine());
    }

    public void MoveToVantage(Transform t)
    {
        StopAllCoroutines();
        followCoroutine = null;        
        vantageCoroutine = StartCoroutine(VantageCoroutine(t));
    }

    IEnumerator VantageCoroutine(Transform t)
    {
        while(true)
        {
            Vector3 targetPosition = t.position;
            targetPosition.x = target.position.x;

            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, t.rotation, lerpSpeed);

            yield return null;
        }
    }
    
    public void ResumeFollow()
    {
        StopAllCoroutines(); 
        vantageCoroutine = null;
        followCoroutine = StartCoroutine(FollowCoroutine());
    }

    IEnumerator FollowCoroutine()
    {
        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, lerpSpeed);

            yield return null;
        }
    }
}
