using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag(Tags.Player)) collider.SendMessage("ApproachedDoor", this, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag(Tags.Player)) collider.SendMessage("LeftDoor", this, SendMessageOptions.DontRequireReceiver);
    }

    public void openDoor()
    {
        Destroy(gameObject);
    }

    public void closeDoor()
    {
    }

}
