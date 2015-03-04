using UnityEngine;
using System.Collections;
using System;

// Scene control scripts use this for some common behavior
public class Scene : Photon.MonoBehaviour
{
    protected void LoadLevel(string level, Action onFinished = null)
    {
        StartCoroutine(LoadLevelCoroutine(level, onFinished));
    }

    IEnumerator LoadLevelCoroutine(string level, Action onFinished)
    {
        yield return Application.LoadLevelAsync(level);

        if (onFinished != null) onFinished();
    }
}
