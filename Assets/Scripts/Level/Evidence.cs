﻿using UnityEngine;
using System.Collections;

// A piece of evidence
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
}
