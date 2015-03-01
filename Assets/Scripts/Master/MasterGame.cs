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
        int id = PhotonNetwork.AllocateViewID();
        photonView.viewID = id;
        network.RaiseCustomEvent(CustomEvent.InitiateMasterControl, id);
    }

    void RequestLaunchSession(MasterSession @session)
    {
        this.session = @session;
        this.session.onFinished += RequestSessionFinish;
        photonView.RPC("LaunchSession", PhotonTargets.All, session.Code);
    }

    [RPC]
    void LaunchSession(int sessionCode)
    {
        lobby.Exit();
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
        lobby.Enter();
    }

}
