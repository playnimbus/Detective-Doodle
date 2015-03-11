using UnityEngine;
using System;

// Hooks and utility for a room
public class LevelRoom : MonoBehaviour
{
    // Called when someone enters/exits a room
    public Action<LevelRoom, Player> onRoomEnter;
    public Action<LevelRoom, Player> onRoomExit;

    public Transform overheadCameraPosition;

    private Fade roomCoverFade;

    void Start()
    {
        roomCoverFade = GetComponentInChildren<Fade>();
        TriggerListener listener = GetComponentInChildren<TriggerListener>();
        listener.onTriggerEntered += OnRoomEnter;
        listener.onTriggerExited += OnRoomExit;
    }

    void OnRoomEnter(Collider coll)
    {
        Player player = coll.GetComponent<Player>();
        if (player != null && onRoomEnter != null) onRoomEnter(this, player);
    }

    void OnRoomExit(Collider coll)
    {
        Player player = coll.GetComponent<Player>();
        if (player != null && onRoomExit != null) onRoomExit(this, player);
    }

    public void Reveal() { roomCoverFade.FadeOut(); }
    public void Conceal() { roomCoverFade.FadeIn(); }

}