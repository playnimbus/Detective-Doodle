using UnityEngine;
using System.Collections;

// Defines custom events that don't rely on a specific PhotonView but use PhotonNetwork.RaiseEvent instead
public enum CustomEvent : byte
{
    GameCommunicatorCreated = 1
}