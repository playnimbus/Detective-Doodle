using UnityEngine;
using System.Collections;
using System;

// Base class for lobby logic
public abstract class Lobby : Scene 
{
    // Set up lobby here
    public abstract void Enter();

    // Clean up things on leave
    public abstract void Exit();
}
