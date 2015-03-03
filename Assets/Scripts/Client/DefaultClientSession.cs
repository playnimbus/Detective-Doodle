using UnityEngine;
using System.Collections;

public class DefaultClientSession : ClientSession
{
    public override void Launch()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        AsyncOperation aop = Application.LoadLevelAsync("Session");
        aop.allowSceneActivation = true;
        yield return aop;
    }

    public override void Finish()
    {

    }
}