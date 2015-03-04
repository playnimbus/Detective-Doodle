using UnityEngine;
using System.Collections;

public class DefaultClientSession : Session
{
    public override void Launch()
    {
        LoadLevel("Session", LevelLoaded);
    }

    public void LevelLoaded()
    {
        photonView.RPC("PlayerCheckIn", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
    }

    public override void Finish()
    {

    }
}