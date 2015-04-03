using UnityEngine;
using System.Collections;
using System;

public static class SceneUtils
{
    // Loads the specified level if it's not already loaded and calls onFinished when finished if provided
    public static void LoadLevel(this MonoBehaviour mono, string level, Action onFinished = null)
    {
        mono.StartCoroutine(LoadLevelCoroutine(level, onFinished));
    }

    public static IEnumerator LoadLevelCoroutine(string level, Action onFinished)
    {
        if (Application.loadedLevelName != level)
            yield return Application.LoadLevelAsync(level);

        if (onFinished != null) onFinished();
    }
}
