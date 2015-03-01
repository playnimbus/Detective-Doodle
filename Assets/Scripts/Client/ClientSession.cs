using UnityEngine;
using System.Collections;

// Client session. Run each client's logic here.
// Will have to sync starting after levels have loaded.
public class ClientSession : Session
{
    public override void Launch()
    {
        Application.LoadLevel("Session");
    }

    public override void Finish()
    {

    }
}
