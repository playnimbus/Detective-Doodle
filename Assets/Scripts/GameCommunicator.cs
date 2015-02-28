using UnityEngine;
using System;

// This will act as a communication portal between game managers
// Defines all things that need to said between master and clients (on the game manager level)
// This will require a PhotonView and work via RPCs. 
[RequireComponent(typeof(PhotonView))]
public class GameCommunicator : Photon.MonoBehaviour
{
    public void MoveToLobby()
    {
        
    }

    [RPC]
    void MoveToLobbyRPC()
    {

    }
}
