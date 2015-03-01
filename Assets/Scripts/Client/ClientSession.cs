using UnityEngine;
using System.Collections;

// Client session. Run each client's logic here.
// Will have to sync starting after levels have loaded.
public class ClientSession : Session
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
