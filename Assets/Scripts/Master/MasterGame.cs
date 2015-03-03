using UnityEngine;
using System;
using System.Collections;

// Master client specific game state management logic
public class MasterGame : Game
{
    private MasterLobby lobby;
    private MasterSession session;

    protected override void Start()
    {
        message = "Master";

        base.Start();
        lobby = gameObject.AddComponent<MasterLobby>();

        network.onConnected += network.CreateRoom;
        network.onCreateRoomFailed += network.CreateRoom;
        network.onCreatedRoom += InitiateControl;
        network.onCreatedRoom += lobby.Enter;

        lobby.onLaunchSession += RequestLaunchSession;        
    }

    void InitiateControl()
    {
        // Take control of the game
        photonView.TransferOwnership(PhotonNetwork.player.ID);
        network.RaiseCustomEvent(CustomEvent.InitiateMasterControl, PhotonNetwork.player.ID);
    }

    void RequestLaunchSession(SessionType type)
    {
        photonView.RPC("LaunchSession", PhotonTargets.All, type);
    }

    [RPC]
    void LaunchSession(SessionType type)
    {
        lobby.Exit();

        switch (type)
        {
            case SessionType.Default:
            default:
                session = gameObject.AddComponent<MasterSession>();
                break;
        }

        session.onFinished += RequestSessionFinish;
        session.Launch(); 
    }

    void RequestSessionFinish()
    {
        this.session.onFinished -= RequestSessionFinish;
        photonView.RPC("FinishSession", PhotonTargets.All);
    }

    [RPC]
    void FinishSession()
    {
        session.Finish();
        Destroy(session);
        session = null;
        lobby.Enter();
    }

}
