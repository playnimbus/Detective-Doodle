using UnityEngine;
using System.Collections;

// Client specific game state management logic
public class ClientGame : Game
{
    private ClientLobby lobby;
    private ClientSession session;

    protected override void Start()
    {
        message = "Client";

        base.Start();
        lobby = gameObject.AddComponent<ClientLobby>();

        network.onConnected += network.JoinRoom;        
        network.onJoinRoomFailed += network.JoinRoom;
        network.onJoinedRoom += lobby.Enter;
        network.onInitiateMasterControl += InitiateMasterControl;
    }
    
    void InitiateMasterControl(int masterID)
    {
        // Assign our PhotonView over to the master
        photonView.viewID = masterID;
    }

    [RPC]
    void LaunchSession(int sessionCode)
    {
        lobby.Exit();

        switch(sessionCode)
        {
            default:
                session = gameObject.AddComponent<ClientSession>();
                break;
        }

        session.Launch();
    }

    [RPC]
    void FinishSession()
    {
        session.Finish();
        lobby.Enter();
    }
}
