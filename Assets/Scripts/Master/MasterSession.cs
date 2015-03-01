using UnityEngine;
using System;

// Masters session. Run all authoritative/master logic here.
public class MasterSession : Session
{
    public Action onFinished;
    public virtual int Code { get { return 0; } }

    public override void Launch()
    {
        Application.LoadLevel("Session");
    }

    public override void Finish()
    {
        
    }
}
