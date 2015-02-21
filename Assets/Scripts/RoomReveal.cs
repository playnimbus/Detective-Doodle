using UnityEngine;
using System.Collections;

public class RoomReveal : MonoBehaviour {

    public GameObject RoomCover;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player0")
        {
            RoomCover.SetActive(false);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player0")
        {
            RoomCover.SetActive(true);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.name == "Player0")
        {
            RoomCover.SetActive(false);
        }
    }
}
