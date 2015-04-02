using UnityEngine;
using UnityEditor;
using System.Collections;

public static class MiscEditor
{

    [MenuItem("Settings / Create Network Settings")]
    private static void CreateNetworkSettings()
    {
        var obj = ScriptableObject.CreateInstance<NetworkSettings>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NetworkSettings.asset");
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Level Batch / Toggle Room Covers")]
    private static void BuildAll()
    {
        GameObject[] Covers = GameObject.FindGameObjectsWithTag("RoomCover");
        for (int i = 0; i < Covers.Length; i++)
        {
            Renderer coverRend = Covers[i].GetComponent<Renderer>();
            coverRend.enabled = !coverRend.enabled;
        }
    }
}