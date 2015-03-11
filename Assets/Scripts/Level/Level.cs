using UnityEngine;
using System;

// Manages level wide stuff that can be used by both Master and Client,
// like individual rooms
public class Level : MonoBehaviour
{
    public event Action<LevelRoom, Player> onEnteredRoom
    {
        add
        {
            LevelRoom[] rooms = GetComponentsInChildren<LevelRoom>();
            foreach(LevelRoom room in rooms)
                room.onRoomEnter += value;
        }
        remove
        {
            LevelRoom[] rooms = GetComponentsInChildren<LevelRoom>();
            foreach (LevelRoom room in rooms)
                room.onRoomEnter -= value;
        }
    }

    public event Action<LevelRoom, Player> onExitedRoom
    {
        add
        {
            LevelRoom[] rooms = GetComponentsInChildren<LevelRoom>();
            foreach (LevelRoom room in rooms)
                room.onRoomExit += value;
        }
        remove
        {
            LevelRoom[] rooms = GetComponentsInChildren<LevelRoom>();
            foreach (LevelRoom room in rooms)
                room.onRoomExit -= value;
        }
    }
}
