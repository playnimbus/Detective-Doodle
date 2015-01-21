using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public GameObject PlayerToFollow;
    Vector3 cameraOffset = new Vector3(0, 15, 0);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        gameObject.transform.position = PlayerToFollow.transform.position + cameraOffset;
	
	}
}
