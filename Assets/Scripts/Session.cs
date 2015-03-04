using UnityEngine;
using System;

// Base class for gameplay sessions.
// Options for further implementing: either subclass this and instantiate new session each time,
// or create a new component that plugs into this object and subclass that
public abstract class Session : Photon.MonoBehaviour
{
    public Action onFinished;
    public abstract void Launch();
    public abstract void Finish();
}
