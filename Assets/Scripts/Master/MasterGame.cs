using UnityEngine;
using System;
using System.Collections;

// Master client specific game state management logic
public class MasterGame : Game
{
    private MasterLobby lobby;
    

    protected override void Start()
    {
        message = "Master";
        base.Start();
        network.onConnected += network.CreateRoom;
        network.onCreateRoomFailed += network.CreateRoom;
        network.onCreatedRoom += InitiateControl;
        network.onCreatedRoom += () => { photonView.RPC("GoToLobby", PhotonTargets.OthersBuffered); };
    }
    
    void InitiateControl()
    {
        int id = PhotonNetwork.AllocateViewID();
        photonView.viewID = id;
        network.RaiseCustomEvent(CustomEvent.InitiateMasterControl, id);
    }

    [RPC]
    void GoToLobby()
    {
        lobby = gameObject.AddComponent<MasterLobby>();
        lobby.onStartGame += StartGame;
        lobby.Enter();
    }

    void StartGame(object gameInfo)
    {

    }  
}
