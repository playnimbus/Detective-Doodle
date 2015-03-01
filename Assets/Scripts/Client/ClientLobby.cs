using UnityEngine;
using System.Collections;

// Clients lobby logic. Whatever clients do in a lobby will be done here (like character customization)
public class ClientLobby : Lobby
{
    public override void Enter()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        AsyncOperation aop = Application.LoadLevelAsync("ClientLobby");
        aop.allowSceneActivation = true;
        yield return aop;
    }

    public override void Exit()
    {
        
    }
}
