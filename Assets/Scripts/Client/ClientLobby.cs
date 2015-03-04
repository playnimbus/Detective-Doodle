using UnityEngine;
using System.Collections;

// Clients lobby logic. Whatever clients do in a lobby will be done here (like character customization)
public class ClientLobby : Lobby
{
    public override void Enter()
    {
        LoadLevel("ClientLobby");
    }

    public override void Exit()
    {
        
    }
}
