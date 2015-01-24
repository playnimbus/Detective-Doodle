﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : Photon.MonoBehaviour {

    public GameObject bystanderPlayer;
    public GameObject murdererPlayer;
    public GameObject vigilantePlayer;

    public GameObject connectBtn;
    public GameObject hostBtn;

    bool isHost = false;

    private const string roomName = "RoomName";

	// Use this for initialization
	void Start () {
       PhotonNetwork.ConnectUsingSettings(".1");
	}
	
	// Update is called once per frame
	void Update () {
	    if(isHost)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                PhotonNetwork.DestroyAll();
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.NetworkStatisticsReset();
                PhotonNetwork.LoadLevel(Application.loadedLevel);
            }
        }
	}

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        GUI.Label(new Rect(10, 10, 230, 70), "Players Connected: " + PhotonNetwork.countOfPlayers);
        GUI.Label(new Rect(10, 30, 230, 70), "Ping: " + PhotonNetwork.GetPing());
    }

    public void HostRoom()
    {
        PhotonNetwork.CreateRoom("DoodleDetective");
        isHost = true;
    }
    public void ConnectRoom()
    {
        PhotonNetwork.JoinRoom("DoodleDetective");
    }

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("Failed to Connect");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected");

        if (isHost == false)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            GameObject tempPlayer = PhotonNetwork.Instantiate("bystanderPlayer", new Vector3(12, -16, 50), Quaternion.identity, 0);
            tempPlayer.name = "Player" + playerList.Length;
            //tempPlayer.GetComponentInChildren<SwordScript>().playerName = tempPlayer.name;

            GameObject bystanderCamera = (GameObject)Instantiate(Resources.Load("bystanderCamera"));

            bystanderCamera.transform.position = tempPlayer.transform.position + new Vector3(0, 20, 0);

            tempPlayer.GetComponent<playerController>().playerThumbpad = bystanderCamera.GetComponent<CameraFollow>().playerThumbad;
            tempPlayer.GetComponent<playerController>().playerCamera = bystanderCamera.GetComponent<Camera>();
            bystanderCamera.GetComponent<CameraFollow>().playerToFollow = tempPlayer.transform;
            

        }
        else
        {
            GameObject hostCamera = (GameObject)Instantiate(Resources.Load("HostCamera"));
            hostCamera.transform.position = new Vector3(2.5f, 32.87f, 57.2f);
        }

        connectBtn.SetActive(false);
        hostBtn.SetActive(false);
    }
}
