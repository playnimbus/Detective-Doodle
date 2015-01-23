﻿using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {

    public GameObject bystanderPlayer;
    public GameObject Murderer;
    public GameObject Vigilante;

    private const string roomName = "RoomName";

	// Use this for initialization
	void Start () {
       PhotonNetwork.ConnectUsingSettings(".1");
       
       //Bystander.name = "Bystander";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            //Create Room
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
            {
                PhotonNetwork.CreateRoom("DoodleDetective");
            }
            //Join Room
            else
            {
                PhotonNetwork.JoinRoom("DoodleDetective");
            }
        }
    }

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("Failed to Connect to Room");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        GameObject tempPlayer =  PhotonNetwork.Instantiate("bystanderPlayer", new Vector3(12, -16, 50), Quaternion.identity, 0);
        //GameObject tempCamera =  PhotonNetwork.Instantiate("bystanderCamera", new Vector3(12, -3.72F, 50), Quaternion.identity, 0);
        GameObject bystanderCamera = (GameObject)Instantiate(Resources.Load("bystanderCamera"));

        //bystanderCamera.transform.Rotate(new Vector3(90, 0, 0));

        tempPlayer.GetComponent<playerController>().playerCamera = bystanderCamera.GetComponent<Camera>();
        bystanderCamera.GetComponent<CameraFollow>().playerToFollow = tempPlayer.transform;
    }
}
