using UnityEngine;
using System;
using System.Collections;

// Master clients lobby logic. Will include things like picking game mode and other game wide settings
public class MasterLobby : Lobby
{
    private MasterLobbyMenu menu;

    // This gets activated when the session is to start
    public Action<MasterSession> onLaunchSession;

    public override void Enter()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        AsyncOperation aop = Application.LoadLevelAsync("MasterLobby");
        aop.allowSceneActivation = true; 
        yield return aop;

        menu = FindObjectOfType<MasterLobbyMenu>();
        menu.launchSessionClicked += LaunchSession;
    }

    public void LaunchSession()
    {
        MasterSession session = gameObject.AddComponent<MasterSession>();
        if (onLaunchSession != null) onLaunchSession(session);
    }

    public override void Exit()
    {
        menu = null;
    }
}
