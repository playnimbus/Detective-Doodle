using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{   
    [Range(0, 1)]
    public float lerpSpeed;

    private Transform target;
    private Vector3 offset;

    public void Init(Transform target)
    {
        this.target = target;
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpSpeed);
    }
}
