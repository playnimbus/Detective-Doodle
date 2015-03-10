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

        menu = FindObjectOfType<SessionMenu>();
        menu.buttonClicked += RequestFinish;

        foreach (PhotonPlayer p in PhotonNetwork.otherPlayers)
            photonView.RPC("CreatePlayer", p, new Vector3(0, 2.5f, 0));

        StartCoroutine(MenuCoroutine());
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