using UnityEngine;
using System.Collections;

// Master clients lobby logic. Will include things like picking game mode
// and other game wide settings
public class MasterLobbyManager : LobbyManager
{
    public override void LoadScene()
    {
        // Temporary no op
        // Application.LoadLevel("Lobby");
    }
}
