using UnityEngine;
using System.Collections;

public class WhodunnitClientSession : Session
{
    private Level level;
    private Player player;
    private int numRoomsPlayerIsIn;

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
    }

    [RPC]
    void MakeMurderer()
    {
        player.MakeMurderer();
    }

    public override void Finish()
    {

    }
}