using UnityEngine;
using System.Collections;

public class Key : Item
{
    public override void Use(Player target)
    {
       // target.photonView.RPC("Kill", PhotonTargets.All);
    }

    public override string ItemName
    {
        get { return "Key"; }
    }

    public override string ResourceName
    {
        get { return "Key"; }
    }

    public override Vector3 LocalPositionOnPlayer
    {
        get { return new Vector3(0.26953f, 1.5f, 0.98f); }
    }

    public override Vector3 LocalRotationOnPlayer
    {
        get { return new Vector3(90, 340, 0); }
    }

}