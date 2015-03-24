using UnityEngine;
using System.Collections;

public class WhodunnitClientSession : Session
{
    private Level level;
    private Player player;
    private int numRoomsPlayerIsIn;

    public override void Launch()
    {
        LoadLevel("Sessionmike", LevelLoaded);
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
    void AssignMurderer()
    {
        player.photonView.RPC("MakeMurderer", PhotonTargets.All);
    }

    [RPC]
    void AssignDetective()
    {
        player.photonView.RPC("MakeDetective", PhotonTargets.All);
    }

    public override void Finish()
    {

    }
}