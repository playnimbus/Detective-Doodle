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

}