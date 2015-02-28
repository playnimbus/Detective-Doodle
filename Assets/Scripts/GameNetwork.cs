using UnityEngine;
using System;
using Recievers = ExitGames.Client.Photon.Lite.ReceiverGroup;

// Game network provides static network functionality and wraps
// all network events (not game relatated, like OnDisconnected) in C# events to listen on
// Owned and created by Game
public class GameNetwork : MonoBehaviour
{
    // Temporary setting strings
    private readonly string roomName = "lksjfdalkjhfdlkjdsag";
    private readonly string version = "0.1";
        
    // Shared events
    public Action onConnected;

    // Master events
    public Action onCreatedRoom;
    public Action onCreateRoomFailed;

    // Client events
    public Action onJoinedRoom;
    public Action<int> onInitiateMasterControl;
    public Action onJoinRoomFailed;

	void Start () 
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings(version);
        PhotonNetwork.OnEventCall += OnCustomEvent;
	}

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void RaiseCustomEvent(CustomEvent @event, object content = null, Recievers recievers = Recievers.Others)
    {
        // This option makes sure its buffered
        RaiseEventOptions opts = new RaiseEventOptions();
        opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache;

        PhotonNetwork.RaiseEvent((byte)@event, content, true, opts);       
    }

    private void OnCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == (byte)CustomEvent.InitiateMasterControl)
        {
            if (onInitiateMasterControl != null) onInitiateMasterControl((int)content);
        }
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

    void OnPhotonCreateRoomFailed()
    {
        if (onCreateRoomFailed != null) onCreateRoomFailed();
    }

    void OnJoinedRoom()
    {
        if (onJoinedRoom != null) onJoinedRoom();
    }

    void OnPhotonJoinRoomFailed()
    {
        if (onJoinRoomFailed != null) onJoinRoomFailed();
    }
}
