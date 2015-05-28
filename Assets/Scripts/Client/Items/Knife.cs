using UnityEngine;
using System.Collections;

public class Knife : Item
{
    public override void Use(Player target)
    {
        target.photonView.RPC("Kill", PhotonTargets.All);
    }

    public override string ItemName
    {
        get { return "Knife"; }
    }

    public override string ResourceName
    {
        get { return ""; }
    }


}