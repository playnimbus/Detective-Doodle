using UnityEngine;
using System;

// Master clients lobby logic. Will include things like picking game mode and other game wide settings
public class MasterLobby : Lobby
{
    // This gets activated when the session is to start
    public Action<MasterSession> onLaunchSession;

    public override void Enter()
    {
        Application.LoadLevel("MasterLobby");
    }

    public override void Exit()
    {

    }
}
