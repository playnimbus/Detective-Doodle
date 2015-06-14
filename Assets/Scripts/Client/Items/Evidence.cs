using UnityEngine;
using System.Collections;

public class Evidence : Item
{

    public override void Use(Player target)
    {
        // target.photonView.RPC("Kill", PhotonTargets.All);
    }

    public override string ItemName
    {
        get { return "Evidence"; }
    }

    public override string ResourceName
    {
        get { return "Evidence"; }
    }

    public override Vector3 LocalPositionOnPlayer
    {
        get { return new Vector3(14.6f, 20.4f, -1.4f); }
    }

    public override Vector3 LocalRotationOnPlayer
    {
        get { return new Vector3(-54.380f, 50, 8.35104f); }
    }
}
