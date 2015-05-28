using UnityEngine;
using System.Collections;

public class MonsterClientSession : Session
{
    private Level level;
    private PlayerMonster player;
    private AudioBank audio;

    public override void Launch()
    {
        this.LoadLevel("Monster", LevelLoaded);
    }

    public void LevelLoaded()
    {
        InitAudio();
        photonView.RPC("PlayerCheckIn", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
        level = FindObjectOfType<Level>();
    }

    void InitAudio()
    {
        GameObject audioResource = Resources.Load<GameObject>("ClientAudio");
        GameObject audioGO = Instantiate(audioResource);
        audio = audioGO.GetComponent<AudioBank>();
    }

    [RPC]
    void CreatePlayer(Vector3 location)
    {
        GameObject playerGO = PhotonNetwork.Instantiate("PlayerMonster", location, Quaternion.identity, 0);
        player = playerGO.GetComponent<PlayerMonster>();
        player.action += OnPlayerAction;
        player.Audio = audio;
    }

    [RPC]
    void AssignMonster()
    {
        player.photonView.RPC("MakeMonster", PhotonTargets.All);
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
      //  player.BystandersWon();
    }

    [RPC]
    void MurdererWon()
    {
   //     player.MurdererWon();
    }

    public override void Finish()
    {
        player.action -= OnPlayerAction;
    }
}