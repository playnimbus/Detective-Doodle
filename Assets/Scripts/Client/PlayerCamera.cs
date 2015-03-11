using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{   
    [Range(0, 1)]
    public float lerpSpeed;

    private Transform target;
    private Vector3 offset;
    private Quaternion initialRotation;

    public void Init(Transform target)
    {
        this.target = target;
        offset = transform.position - target.position;
        initialRotation = transform.rotation;
        StartCoroutine(FollowCoroutine());
    }

    public void MoveToTransform(Transform t)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToTransformCoroutine(t));
    }

    IEnumerator MoveToTransformCoroutine(Transform t)
    {
        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, t.position, lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, t.rotation, lerpSpeed);

            yield return null;
        }
    }
    
    public void ResumeFollow()
    {
        StopAllCoroutines();
        StartCoroutine(FollowCoroutine());
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
