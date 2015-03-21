using UnityEngine;
using System;

// Manages level wide stuff that can be used by both Master and Client,
// like individual rooms
public class Level : MonoBehaviour
{
    LevelRoom[] rooms;

    void Start()
    {
        rooms = GetComponentsInChildren<LevelRoom>();
    }
}
