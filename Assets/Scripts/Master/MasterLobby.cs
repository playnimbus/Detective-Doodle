﻿using UnityEngine;
using System;
using System.Collections;

// Master clients lobby logic. Will include things like picking game mode and other game wide settings
public class MasterLobby : Lobby
{
    private MasterLobbyMenu menu;

    // This gets activated when the session is to start
    public Action<SessionType> onLaunchSession;

    public override void Enter()
    {
        Action levelLoaded = () =>
            {
                menu = FindObjectOfType<MasterLobbyMenu>();
                menu.launchSessionClicked += LaunchSession;
            };

        LoadLevel("MasterLobby", levelLoaded);
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
}