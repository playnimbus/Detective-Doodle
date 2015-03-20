using UnityEngine;
using System;
using System.Collections;

// Master clients lobby logic. Will include things like picking game mode and other game wide settings
public class MasterLobby : Lobby
{
    private MasterLobbyMenu menu;
    private string roomName;

    // This gets activated when the session is to start
    public Action<SessionType> onLaunchSession;

    public override void Enter()
    {
        LoadLevel("MasterLobby", LevelLoaded);
    }

    void LevelLoaded()
    {
        menu = FindObjectOfType<MasterLobbyMenu>();
        menu.launchSessionClicked += LaunchSession;
        menu.SetRoomName(roomName);
        menu.SetNetwork(GetComponent<GameNetwork>());
    }

    public void LaunchSession()
    {
        if (onLaunchSession != null) onLaunchSession(SessionType.Default);
    }

    public override void Exit()
    {
        menu.launchSessionClicked -= LaunchSession;
        menu = null;
    }

    public void SetRoomName(string name)
    {
        this.roomName = name;
        if (menu != null) menu.SetRoomName(name);
    }
}
