using UnityEngine;
using System;

// Base class for gameplay sessions.
// Options for further implementing: either subclass this and instantiate new session each time,
// or create a new component that plugs into this object and subclass that
public abstract class Session : Photon.MonoBehaviour
{
    // Activate to let Game know we've finished
    public Action onFinished;

    // Set the session going
    public abstract void Launch();

    // Let the session clean up
    public abstract void Finish();

    void Update()
    {
        Analytics.RoundTimer += Time.deltaTime;
    }
}
