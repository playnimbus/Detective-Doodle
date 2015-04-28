using UnityEngine;
using System;
using Recievers = ExitGames.Client.Photon.Lite.ReceiverGroup;

// Game network provides static network functionality and wraps
// all network events (not game relatated, like OnDisconnected) in C# events to listen on
// Owned and created by Game
public class GameNetwork : MonoBehaviour
{
    // The settings asset
    [HideInInspector]
    public NetworkSettings settings;

    public Action<int> onInitiateMasterControl;

	void Start () 
    {
        settings = Resources.Load<NetworkSettings>("Network/NetworkSettings");

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.OnEventCall += OnCustomEvent;

//#if !UNITY_IPHONE

        if (settings.local)
        {
            if (settings.localSettings.connectAutomatically)
            {
                PhotonNetwork.ConnectToMaster(settings.localSettings.localIP, PhotonNetwork.PhotonServerSettings.ServerPort, PhotonNetwork.PhotonServerSettings.AppID, settings.version);
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(settings.version);
        }
//#else

		//PhotonNetwork.ConnectUsingSettings(settings.version);

//#endif

    }

    public void ConnectWithIP(string ip)
    {
        PhotonNetwork.ConnectToMaster(ip, PhotonNetwork.PhotonServerSettings.ServerPort, PhotonNetwork.PhotonServerSettings.AppID, settings.version);
        PlayerPrefs.SetString("lastIp", ip);
        print(ip + " has been saved");
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

    public void RaiseCustomEvent(byte eventCode, object content = null, Recievers recievers = Recievers.Others)
    {
        // This option makes sure its buffered
        RaiseEventOptions opts = new RaiseEventOptions();
        opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache;

        PhotonNetwork.RaiseEvent(eventCode, content, true, opts);       
    }

    private void OnCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == CustomEvent.InitiateMasterControl)
        {
            if (onInitiateMasterControl != null) onInitiateMasterControl((int)content);
        }
    }

}
