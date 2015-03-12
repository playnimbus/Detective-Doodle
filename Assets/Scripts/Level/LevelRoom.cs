using UnityEngine;
using System;

// Hooks and utility for a room
public class LevelRoom : MonoBehaviour
{
    // Called when someone enters/exits a room
    public Action<LevelRoom, Player> onRoomEnter;
    public Action<LevelRoom, Player> onRoomExit;

    // Vantage point for the camera to go to when player enters
    public Transform overheadCameraPosition;

    // Fades the cover for a room
    private Fade roomCoverFade;
    
    // Monitoring evidence stashes
    private EvidenceStash[] stashes;
    public event Action<EvidenceStash, Player> onPlayerApproachStash
    {
        add
        {
            for (int i = 0; i < stashes.Length; i++)
                stashes[i].onPlayerApproach += value;
        }
        remove
        {
            for (int i = 0; i < stashes.Length; i++)
                stashes[i].onPlayerApproach -= value;
        }
    }
    public event Action<EvidenceStash, Player> onPlayerLeaveStash
    {
        add
        {
            for (int i = 0; i < stashes.Length; i++)
                stashes[i].onPlayerLeave += value;
        }
        remove
        {
            for (int i = 0; i < stashes.Length; i++)
                stashes[i].onPlayerLeave -= value;
        }
    }

    void Start()
    {
        stashes = GetComponentsInChildren<EvidenceStash>();
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