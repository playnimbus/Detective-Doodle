using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{   
    [Range(0, 1)]
    public float lerpSpeed;

    private Transform target;
    private Transform initialTarget;
    private Vector3 offset;
    private Vector3 initialOffset;
    private Quaternion rotation;
    private Quaternion initialRotation;

    private Coroutine followCoroutine;
    private Coroutine vantageCoroutine;

	public Vector3 playerDirection = Vector3.zero;
    private Vector3 playerDirectionInfluence = Vector3.zero;

    public void Init(Transform target)
    {
        initialTarget = target;
        this.target = initialTarget;
        initialOffset  = transform.position - target.position;
        offset = initialOffset;
        initialRotation = transform.rotation;
        rotation = initialRotation;
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
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while(true)
        {
            playerDirectionInfluence = Vector3.Lerp(playerDirectionInfluence, playerDirection, 0.1f);

            transform.position = Vector3.Lerp(transform.position, target.position + offset + (playerDirectionInfluence * 2.5f), lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, lerpSpeed);

            yield return wait;
        }
    }

    public void BringCloserToPosition(Transform position)
    {
        target = position;
        offset *= 0.92f;
    }

    public void RestoreDistance()
    {
        offset = initialOffset;
        rotation = initialRotation;
        target = initialTarget;
    }
}
