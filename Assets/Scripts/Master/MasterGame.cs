using UnityEngine;
using System.Collections.Generic;
using System;



// Master client specific game state management logic
public class MasterGame : Game
{
    private MasterLobby Lobby { get { return base.lobby as MasterLobby; } }
    private List<PhotonPlayer> playersInSession;
    private string roomName;

    protected void Start()
    {
        base.Start();
        base.lobby = gameObject.AddComponent<MasterLobby>();

        network.onConnected += CreateRoom;
        network.onCreateRoomFailed += CreateRoom;
        network.onCreatedRoom += InitiateControl;
        network.onCreatedRoom += lobby.Enter;
        network.onCreatedRoom += () => { Lobby.SetRoomName(roomName); };
        network.onInitiateMasterControl += InitiateMasterControl;
        network.onPlayerConnected += PlayerJoined;

        Lobby.onLaunchSession += RequestLaunchSession;
    }

    void CreateRoom()
    {
        char[] chars = new char[4];
        for(int i=0; i<chars.Length; i++)
            chars[i] = (char)UnityEngine.Random.Range((int)'A', (int)'Z' + 1);

        roomName = new string(chars);
        network.CreateRoom(roomName);
    }

    void InitiateControl()
    {
        // Take control of the game
        network.RaiseCustomEvent(CustomEvent.InitiateMasterControl, PhotonNetwork.player.ID);
    }
    
    protected override Session CreateSession(SessionType type)
    {
        Session session = null;

        switch (type)
        {
            case SessionType.Default:
            default:
                session = gameObject.AddComponent<DefaultMasterSession>();
                break;
        }

        session.onFinished += RequestSessionFinish;
        return session;
    }

    void PlayerJoined(PhotonPlayer player)
    {

    }

    void RequestLaunchSession(SessionType type)
    {
        photonView.RPCEx("LaunchSession", PhotonTargets.All, type);
    }

    void RequestSessionFinish()
    {
        this.session.onFinished -= RequestSessionFinish;
        photonView.RPCEx("FinishSession", PhotonTargets.All);
    }


    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 250, 25), "Ping: " + PhotonNetwork.GetPing());
    }
}
