using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class WhodunnitMasterSession : Session
{
    private SessionMenu menu;
    private int numPendingPlayers;
    private EvidenceStash[] stashes;
    private Level level;
    private int totalPlayers;
    private int deadPlayers;

    public override void Launch()
    {
        deadPlayers = 0;
        totalPlayers = PhotonNetwork.otherPlayers.Length;
        numPendingPlayers = totalPlayers;
        LoadLevel("SessionMike", LevelLoaded);

        Analytics.Initialize(Analytics.GameModes.Detective, numPendingPlayers);
    }

    void RequestFinish()
    {
        if (onFinished != null) onFinished();
    }

    void LevelLoaded()
    {
        StartCoroutine(WaitForPlayers());
        level = FindObjectOfType<Level>();
    }

    IEnumerator WaitForPlayers()
    {
        while (numPendingPlayers > 0)
            yield return null;

        // Done waiting for players, init
        Initialize();
    }

    void Initialize()
    {
        GameObject menuGO = Instantiate(Resources.Load<GameObject>("MasterMenu")) as GameObject;
        menu = menuGO.GetComponent<SessionMenu>();
        menu.buttonClicked += RequestFinish;
        StartCoroutine(MenuCoroutine());

        // Create player characters
        SpawnPoint[] spawnPoints = level.GetSpawnPointsShuffled();
        for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
        {
            if (i > spawnPoints.Length) Debug.LogError("[WhodunnitMasterSession] More players than available spawn points.");
            
            PhotonPlayer p = PhotonNetwork.otherPlayers[i];
            Vector3 position = spawnPoints[i].transform.position;
            photonView.RPC("CreatePlayer", p, position);
        }

         //   Old hard coded spawn locations
         //   photonView.RPC("CreatePlayer", p, new Vector3(20f, 2.5f, -2.5f));     //Session spawn
         //   photonView.RPC("CreatePlayer", p, new Vector3(-8, 2.5f, -22));        //sessionMike spawn

        // Assign the murderer!
        int murdererIndex = UnityEngine.Random.Range(0, PhotonNetwork.otherPlayers.Length);
        PhotonPlayer murderer = PhotonNetwork.otherPlayers[murdererIndex];
        photonView.RPC("AssignMurderer", murderer);

        // Assign the detective!
        int detectiveIndex = (murdererIndex + 1) % PhotonNetwork.otherPlayers.Length;
        PhotonPlayer detective = PhotonNetwork.otherPlayers[detectiveIndex];
        photonView.RPC("AssignDetective", detective);

        // Set up stashes so only one has evidence in it at a time
        stashes = FindObjectsOfType<EvidenceStash>();
        foreach (EvidenceStash stash in stashes)
            stash.evidenceLooted += StashEvidenceLooted;
        stashes[0].photonView.RPC("SetHasEvidence", PhotonTargets.All, true);
    }

    void StashEvidenceLooted(EvidenceStash stash)
    {
        int index = UnityEngine.Random.Range(0, stashes.Length);
        EvidenceStash newStash = stashes[index];
        newStash.photonView.RPC("SetHasEvidence", PhotonTargets.All, true);
    }

    IEnumerator MenuCoroutine()
    {
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                menu.Toggle();

#if UNITY_XBOXONE

            if (XboxOneInput.GetKeyDown(XboxOneKeyCode.GamepadButtonB))
                RequestFinish();

#endif

            yield return null;
        }
    }

    [RPC]
    void PlayerCheckIn(int playerID)
    {
        numPendingPlayers--;
    }

    [RPC]
    void BystandersWon()
    {
        menu.SetHeader("The detectives have won!", true);
        Analytics.SendGameModeEnd(true);
    }

    [RPC]
    void PlayerKilled()
    {
        deadPlayers++;
        if (deadPlayers >= totalPlayers - 1)
            photonView.RPC("MurdererWon", PhotonTargets.All);
    }

    [RPC]
    void MurdererWon()
    {
        menu.SetHeader("The murderer has won!", true);
        Analytics.SendGameModeEnd(false);
    }

    public override void Finish()
    {
        menu.buttonClicked -= RequestFinish;
        menu = null;
    }
}