using UnityEngine;
using System;

// Manages level wide stuff that can be used by both Master and Client,
// like individual rooms
public class Level : MonoBehaviour
{
    LevelRoom[] rooms;

    // Room monitoring    
    public event Action<LevelRoom, Player> onEnteredRoom
    {
        add
        {
            foreach(LevelRoom room in rooms)
                room.onRoomEnter += value;
        }
        remove
        {
            foreach (LevelRoom room in rooms)
                room.onRoomEnter -= value;
        }
    }
    public event Action<LevelRoom, Player> onExitedRoom
    {
        add
        {
            foreach (LevelRoom room in rooms)
                room.onRoomExit += value;
        }
        remove
        {
            foreach (LevelRoom room in rooms)
                room.onRoomExit -= value;
        }
    }

    // Stash monitoring
    public event Action<EvidenceStash, Player> onPlayerApproachStash
    {
        add
        {
            for (int i = 0; i < rooms.Length; i++)
                rooms[i].onPlayerApproachStash += value;
        }
        remove
        {
            for (int i = 0; i < rooms.Length; i++)
                rooms[i].onPlayerApproachStash -= value;
        }
    }
    public event Action<EvidenceStash, Player> onPlayerLeaveStash
    {
        add
        {
            for (int i = 0; i < rooms.Length; i++)
                rooms[i].onPlayerLeaveStash += value;
        }
        remove
        {
            for (int i = 0; i < rooms.Length; i++)
                rooms[i].onPlayerLeaveStash -= value;
        }
    }

    void Start()
    {
        rooms = GetComponentsInChildren<LevelRoom>();
    }
}
