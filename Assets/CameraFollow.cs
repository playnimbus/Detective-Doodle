using UnityEngine;
using System.Collections;

public class CameraFollow : Photon.MonoBehaviour {

    public GameObject playerThumbad;

    public float dampTime = 0.15f;
    private Vector3 cameraOffset = new Vector3(0, 15, 0);
    private Vector3 velocity = Vector3.zero;
    public Transform playerToFollow;

    void Start()
    {

    }

    void Update()
    {
        
        //gameObject.transform.position = playerToFollow.transform.position + cameraOffset;
        if (playerToFollow)
        {
            Vector3 point = camera.WorldToViewportPoint(playerToFollow.position);
            Vector3 delta = playerToFollow.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }
}
