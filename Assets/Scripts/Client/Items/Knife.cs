using UnityEngine;
using System.Collections;

public class Knife : Item
{
    public override void Use(Player target)
    {
        target.photonView.RPC("Kill", PhotonTargets.All);
    }
}