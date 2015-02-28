using UnityEngine;
using System;
using System.Collections;

// Master client specific game state management logic
public class MasterGameManager : GameManager
{
    private string message = "Master";

    private MasterLobbyManager lobbyManager;

    protected override void Start()
    {
        base.Start();
        networkManager.onConnected += networkManager.CreateRoom;
        networkManager.onCreateRoomFailed += networkManager.CreateRoom;

        networkManager.onCreatedRoom += CreateCommunicator;
        networkManager.onCreatedRoom += CreateAndGoToLobby;
    }
    
    void CreateCommunicator()
    {
        int id = PhotonNetwork.AllocateViewID();
        communicator = gameObject.AddComponent<GameCommunicator>();
        communicator.photonView.viewID = id;

        networkManager.RaiseCustomEvent(CustomEvent.GameCommunicatorCreated, id);
    }

    void CreateAndGoToLobby()
    {
        lobbyManager = gameObject.AddComponent<MasterLobbyManager>();
        lobbyManager.LoadScene();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 230, 70), "Message: " + message);
    }
}
