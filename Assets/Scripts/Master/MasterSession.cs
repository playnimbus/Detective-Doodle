using UnityEngine;
using System;
using System.Collections;

// Masters session. Run all authoritative/master logic here.
// Inherit off this and override methods
public abstract class MasterSession : Session
{
    // Activate this when finished
    public Action onFinished;
}
