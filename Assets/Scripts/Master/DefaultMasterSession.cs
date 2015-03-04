using UnityEngine;
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