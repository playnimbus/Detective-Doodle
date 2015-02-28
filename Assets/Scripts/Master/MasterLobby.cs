using UnityEngine;
using System;

// Master clients lobby logic. Will include things like picking game mode and other game wide settings
public class MasterLobby : Lobby
{
    // This gets activated when the game has been chosen
    public Action<object> onStartGame;

    public override void Enter()
    {
        // Temporary no op
        // Application.LoadLevel("Lobby");
    }
}
