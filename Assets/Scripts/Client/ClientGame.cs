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

        network.onConnected += network.JoinRoom;        
        network.onJoinRoomFailed += network.JoinRoom;
        network.onJoinedRoom += lobby.Enter;
        network.onInitiateMasterControl += InitiateMasterControl;
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
