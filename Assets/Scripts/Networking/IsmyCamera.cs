using UnityEngine;
using System.Collections;

public class IsmyCamera : Photon.MonoBehaviour {

    public void Awake()
    {
        if (!photonView.isMine)
        {
            Destroy(gameObject);
            //GetComponentInChildren<Camera>().enabled = false;
            //GetComponentInChildren<AudioListener>().enabled = false;
        }
    }
}
