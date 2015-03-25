using UnityEngine;
using System.Collections;

public class WhodunnitClientSession : Session
{
    private Level level;
    private Player player;

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
        player.action += OnPlayerAction;
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

    void OnPlayerAction(byte action)
    {
        switch (action)
        {
            case Player.PlayerAction.MurdererAccused:
                photonView.RPC("BystandersWon", PhotonTargets.All);
                break;
            case Player.PlayerAction.PlayerKilled:
                photonView.RPC("PlayerKilled", PhotonTargets.MasterClient);
                break;
            default: 
                break;
        }
    }

    [RPC]
    void BystandersWon()
    {
        player.BystandersWon();
    }

    [RPC]
    void MurdererWon()
    {
        player.MurdererWon();
    }

    public override void Finish()
    {
        player.action -= OnPlayerAction;
    }
}