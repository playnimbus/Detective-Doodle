﻿using UnityEngine;
using System.Collections;

public class DefaultClientSession : Session
{
    private Level level;
    private Player player;
    private new PlayerCamera camera;

    public override void Launch()
    {
        LoadLevel("Session", LevelLoaded);
    }

    public void LevelLoaded()
    {
        photonView.RPC("PlayerCheckIn", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
        level = FindObjectOfType<Level>();
    }

    [RPC]
    void CreatePlayer(Vector3 location)
    {
        GameObject playerGO = PhotonNetwork.Instantiate("Player", location, Quaternion.identity, 0);
        player = playerGO.GetComponent<Player>();
        camera = player.Camera;
    }

    public override void Finish()
    {

    }
}