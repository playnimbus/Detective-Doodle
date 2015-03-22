using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditingBatches : EditorWindow{

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
