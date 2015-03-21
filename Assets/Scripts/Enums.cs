using UnityEngine;
using System.Collections;

// Enmuerate all different game types here so the info can be sent via RPC
public static class SessionType
{
    public const byte Whodunnit = 0;
}

// Defines custom events that don't rely on a specific PhotonView but use PhotonNetwork.RaiseEvent instead
public enum CustomEvent : byte
{
    InitiateMasterControl = 0
}

// Tags we are using
public static class Tags
{
    public const string Player = "Player";
}