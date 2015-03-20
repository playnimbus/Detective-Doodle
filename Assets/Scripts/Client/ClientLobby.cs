using UnityEngine;
using System;

// Clients lobby logic. Whatever clients do in a lobby will be done here (like character customization)
public class ClientLobby : Lobby
{
    public Action<string> joinRoomRequested;
    private ClientLobbyMenu menu;
    private bool successfullyJoinedRoom = false;

    public override void Enter()
    {
        LoadLevel("ClientLobby", LevelLoaded);
    }

    void LevelLoaded()
    {
        menu = FindObjectOfType<ClientLobbyMenu>();
        menu.joinRoomRequested += this.joinRoomRequested;
        menu.nameChangedRequested += NameChange;
        menu.SetNetwork(GetComponent<GameNetwork>());
        if (successfullyJoinedRoom) menu.JoinRoomSucceeded();
    }

    public override void Exit()
    {
        menu.joinRoomRequested -= this.joinRoomRequested;
        menu = null;
    }

    public void JoinRoomFailed()
    {
        if (menu != null) menu.JoinRoomFailed();
    }

    public void JoinRoomSucceeded()
    {
        successfullyJoinedRoom = true;
        if (menu != null) menu.JoinRoomSucceeded();
    }

    public void NameChange(string newName)
    {
        PhotonNetwork.player.name = newName;
        PlayerPrefs.SetString("name", newName);
    }
}
