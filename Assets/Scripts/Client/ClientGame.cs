using UnityEngine;
using System.Collections;

// Client specific game state management logic
public class ClientGame : Game
{
    private ClientLobby lobby;
    
    protected void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey("name"))
            PhotonNetwork.player.name = PlayerPrefs.GetString("name");
        else
            PhotonNetwork.player.name = "Anonymous";
        
        network.onInitiateMasterControl += InitiateMasterControl;
    }

    void JoinRoom(string roomName)
    {
        network.JoinRoom(roomName);
    }

    protected override Session CreateSession(byte type)
    {
        switch(type)
        {
            case SessionType.Whodunnit: return gameObject.AddComponent<WhodunnitClientSession>();
            case SessionType.CookieThief: return gameObject.AddComponent<CookieThiefClientSession>();
            case SessionType.Monster: return gameObject.AddComponent<MonsterClientSession>();
            default: return gameObject.AddComponent<WhodunnitClientSession>();
        }
    }

    protected override void EnterLobby()
    {
        SceneUtils.LoadLevel(this, "ClientLobby", OnLobbyLoaded);
    }

    void OnLobbyLoaded()
    {
        lobby = FindObjectOfType<ClientLobby>();
        lobby.SetNetwork(network);
    }


    protected override void ExitLobby()
    {
        lobby = null;
    }
}
