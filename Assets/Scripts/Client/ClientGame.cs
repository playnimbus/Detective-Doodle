using UnityEngine;
using System.Collections;

// Client specific game state management logic
public class ClientGame : Game
{
    private ClientLobby Lobby { get { return base.lobby as ClientLobby; } }
    
    protected void Start()
    {
        base.Start();
        base.lobby = gameObject.AddComponent<ClientLobby>();
        Lobby.joinRoomRequested += JoinRoom;

        PhotonNetwork.player.name = "Adam";

        network.onConnected += lobby.Enter;
        network.onJoinRoomFailed += JoinRoomFailed;
        network.onJoinedRoom += lobby.Enter;
        network.onJoinedRoom += Lobby.JoinRoomSucceeded;
        network.onInitiateMasterControl += InitiateMasterControl;
    }

    void JoinRoom(string roomName)
    {
        network.JoinRoom(roomName);
    }

    void JoinRoomFailed()
    {
        Lobby.JoinRoomFailed();
    }
        
    protected override Session CreateSession(SessionType type)
    {
        switch(type)
        {
            case SessionType.Default:
            default:
                return gameObject.AddComponent<DefaultClientSession>();
        }
    }
}
