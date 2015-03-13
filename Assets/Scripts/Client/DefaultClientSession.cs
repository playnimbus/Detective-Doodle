using UnityEngine;
using System.Collections;

public class DefaultClientSession : Session
{
    private Level level;
    private Player player;
    private new PlayerCamera camera;
    private int numRoomsPlayerIsIn;

    public override void Launch()
    {
        LoadLevel("Session", LevelLoaded);
    }

    public void LevelLoaded()
    {
        photonView.RPC("PlayerCheckIn", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
        level = FindObjectOfType<Level>();
        level.onEnteredRoom += PlayerEnteredRoom;
        level.onExitedRoom += PlayerExitedRoom;
        level.onPlayerApproachStash += PlayerApproachedStash;
        level.onPlayerLeaveStash += PlayerLeftStash;
    }

    [RPC]
    void CreatePlayer(Vector3 location)
    {
        GameObject playerGO = PhotonNetwork.Instantiate("Player", location, Quaternion.identity, 0);
        player = playerGO.GetComponent<Player>();
        camera = player.Camera;
    }

    [RPC]
    void MakeMurderer()
    {
        player.MakeMurderer();
    }

    void PlayerEnteredRoom(LevelRoom room, Player player)
    {
        if (player.photonView.isMine)
        {
            numRoomsPlayerIsIn++;
            camera.MoveToVantage(room.overheadCameraPosition);
            room.Reveal();
        }
    }

    void PlayerExitedRoom(LevelRoom room, Player player)
    {
        if (player.photonView.isMine)
        {
            numRoomsPlayerIsIn--;
            if(numRoomsPlayerIsIn == 0) camera.ResumeFollow();
            room.Conceal();
        }
    }

    void PlayerApproachedStash(EvidenceStash stash, Player player)
    {
        if (player.photonView.isMine)
            player.ApproachedStash(stash);
    }

    void PlayerLeftStash(EvidenceStash stash, Player player)
    {
        if (player.photonView.isMine)
            player.LeftStash(stash);
    }

    public override void Finish()
    {

    }
}