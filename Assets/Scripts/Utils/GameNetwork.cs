using UnityEngine;
using System;
using Recievers = ExitGames.Client.Photon.Lite.ReceiverGroup;

// Game network provides static network functionality and wraps
// all network events (not game relatated, like OnDisconnected) in C# events to listen on
// Owned and created by Game
public class GameNetwork : MonoBehaviour
{
    [Serializable]
    public struct LocalConnectionSettings
    {
        public string localIP;
    }

    public bool useLocalServer;
    public LocalConnectionSettings localSettings;

    // Temporary setting strings
    public string version = "0.1";
        
    // Shared events
    public Action onConnected;
    public Action onPlayerConnected;
    public Action onPlayerDisconnected;
    public Action onPlayerPropertiesChanged;

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
        PhotonNetwork.OnEventCall += OnCustomEvent;

#if !UNITY_IPHONE

        if (useLocalServer)
        {
            PhotonNetwork.ConnectToMaster(localSettings.localIP, PhotonNetwork.PhotonServerSettings.ServerPort, PhotonNetwork.PhotonServerSettings.AppID, version);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(version);
        }
#else

        PhotonNetwork.ConnectUsingSettings(version);

#endif

    }

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("[GameNetwork] Failed to connect to Photon: " + cause);
    }

    public void CreateRoom(string name)
    {
        PhotonNetwork.CreateRoom(name.ToLower());
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name.ToLower());
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

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (onPlayerConnected != null) onPlayerConnected();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        if (onPlayerDisconnected != null) onPlayerDisconnected();
    }

    void OnPhotonPlayerPropertiesChanged(object[] playerAndProperties)
    {
        if (onPlayerPropertiesChanged != null) onPlayerPropertiesChanged();
    }

}
