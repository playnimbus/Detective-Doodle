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
        photonView.TransferOwnership(masterID);
    }

    [RPC]
    void LaunchSession(SessionType type)
    {
        lobby.Exit();

        switch(type)
        {
            case SessionType.Default:
            default:
                session = gameObject.AddComponent<DefaultClientSession>();
                break;
        }

        session.Launch();
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
