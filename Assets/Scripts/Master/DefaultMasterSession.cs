using UnityEngine;
using System.Collections;

public class DefaultMasterSession : Session
{
    private SessionMenu menu;

    public override void Launch()
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        AsyncOperation aop = Application.LoadLevelAsync("Session");
        aop.allowSceneActivation = true;
        yield return aop;

        menu = FindObjectOfType<SessionMenu>();
        menu.buttonClicked += RequestFinish;
    }

    void RequestFinish()
    {
        if (onFinished != null) onFinished();
    }

    public override void Finish()
    {
        menu.buttonClicked -= RequestFinish;
        menu = null;
    }
}