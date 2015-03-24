using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class WhodunnitMasterSession : Session
{
    private SessionMenu menu;
    private int numPendingPlayers;
    private EvidenceStash[] stashes;

    public override void Launch()
    {
        numPendingPlayers = PhotonNetwork.otherPlayers.Length;

        Action levelLoaded = () => { StartCoroutine(WaitForPlayers()); };

        LoadLevel("Session", levelLoaded);

        Analytics.Initialize(Analytics.GameModes.Detective, numPendingPlayers);
    }

    void RequestFinish()
    {
        if (onFinished != null) onFinished();
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
        foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
            photonView.RPC("CreatePlayer", p, new Vector3(20f, 2.5f, -2.5f));

        // Assign the murderer!
        int murdererIndex = UnityEngine.Random.Range(0, PhotonNetwork.otherPlayers.Length);
        PhotonPlayer murderer = PhotonNetwork.otherPlayers[murdererIndex];
        photonView.RPC("AssignMurderer", murderer);

        // Assign the detective!
        int detectiveIndex = (murdererIndex + 1) % PhotonNetwork.otherPlayers.Length;
        PhotonPlayer detective = PhotonNetwork.otherPlayers[detectiveIndex];
        photonView.RPC("AssignDetective", detective);

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

    public override void Finish()
    {
        menu.buttonClicked -= RequestFinish;
        menu = null;
    }
}