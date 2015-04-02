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

        if (PlayerPrefs.HasKey("name"))
            PhotonNetwork.player.name = PlayerPrefs.GetString("name");
        else
            PhotonNetwork.player.name = "Anonymous";

        lobby.Enter();
        network.onJoinRoomFailed += JoinRoomFailed;
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

    protected override Session CreateSession(byte type)
    {
        switch(type)
        {
            case SessionType.Whodunnit:
            default:
                return gameObject.AddComponent<WhodunnitClientSession>();
        }
    }
}
