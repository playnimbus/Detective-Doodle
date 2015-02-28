using UnityEngine;
using System.Collections;

// Client specific game state management logic
public class ClientGame : Game
{
    private ClientLobby lobby;

    protected override void Start()
    {
        base.Start();
        network.onConnected += network.JoinRoom;
        network.onJoinedRoom += JoinedRoom;
        network.onJoinRoomFailed += network.JoinRoom;
        network.onInitiateMasterControl += InitiateMasterControl;

        message = "Client";
    }

    void JoinedRoom()
    {
        lobby = gameObject.AddComponent<ClientLobby>();
        lobby.Enter();
    }

    void InitiateMasterControl(int masterID)
    {
        // Assign our PhotonView over to the master
        photonView.viewID = masterID;
    }
}
