using UnityEngine;
using System;
using System.Collections;

// Masters session. Run all authoritative/master logic here.
public class MasterSession : Session
{
    public Action onFinished;
    public virtual int Code { get { return 0; } }

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
