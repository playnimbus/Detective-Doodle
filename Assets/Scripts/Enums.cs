using UnityEngine;
using System.Collections;

public enum ItemPickups : byte { Nothing = 0, Evidence, Key };

public enum ShirtColors : byte { blue = 0, green = 1, red = 2 };
public enum HatColors : byte { blue = 0, orange = 1, red = 2 };
public enum GloveColors : byte { blue = 0, green = 1, red = 2 };

// Enmuerate all different game types here so the info can be sent via RPC
public static class SessionType
{
    public const byte Whodunnit = 0;
    public const byte CookieThief = 1;
}

// Defines custom events that don't rely on a specific PhotonView but use PhotonNetwork.RaiseEvent instead
public static class CustomEvent
{
    public const byte InitiateMasterControl = 0;
}

// Tags we are using
public static class Tags
{
    public const string Player = "Player";
}