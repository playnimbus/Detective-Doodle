using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class DefaultMasterSession : Session
{
    private SessionMenu menu;
    private int numPendingPlayers;

    public override void Launch()
    {
        numPendingPlayers = PhotonNetwork.otherPlayers.Length;

        Action levelLoaded = () => { StartCoroutine(WaitForPlayers()); };

        LoadLevel("Session", levelLoaded);
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
        int index = UnityEngine.Random.Range(0, PhotonNetwork.otherPlayers.Length);
        PhotonPlayer murderer = PhotonNetwork.otherPlayers[index];
        photonView.RPC("MakeMurderer", murderer);
    }


    IEnumerator MenuCoroutine()
    {
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                menu.Toggle();

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