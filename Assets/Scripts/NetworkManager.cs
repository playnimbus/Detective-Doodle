using UnityEngine;
using System;

public class NetworkManager : MonoBehaviour 
{
    // Default room name
    public string roomName;
        
    // Shared events
    public Action onConnected;

    // Master events
    public Action onCreatedRoom;

    // Client events
    public Action onJoinedRoom;

	void Start () 
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings("0.1");
	}

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom(string room = null)
    {
        PhotonNetwork.JoinRoom(String.IsNullOrEmpty(room) ? roomName : room);
    }

    // Photon events

    void OnConnectedToMaster()
    {
        if (onConnected != null) onConnected();
    }

    void OnCreatedRoom()
    {
        if (onCreatedRoom != null) onCreatedRoom();
    }

    void OnJoinedRoom()
    {
        if (onJoinedRoom != null) onJoinedRoom();
    }
}
