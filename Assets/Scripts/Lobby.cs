using UnityEngine;
using System.Collections;

// Base class for lobby logic
public abstract class Lobby : Photon.MonoBehaviour 
{
    public abstract void Enter();
    public abstract void Exit();
}
