using UnityEngine;
using UnityEditor;
using System.Collections;

public class Build
{
    static string buildFolder = "Builds/";
    static string sceneFolder = "Assets/Scenes/";

    static string[] masterScenes = { "MasterLobby.unity" };
    static string[] clientScenes = { "ClientLobby.unity" };
    static string[] sharedScenes = { "SessionMike.unity" };

    [MenuItem("Build / Create Network Settings")]
    private static void CreateNetworkSettings()
    {
        var obj = ScriptableObject.CreateInstance<NetworkSettings>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NetworkSettings");
        AssetDatabase.CreateAsset(obj, path);

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Build / Master PC")]
    private static void BuildMasterPC()
    {
        string[] scenes = CombineScenesAndAddFolder(masterScenes, sharedScenes);

        BuildPipeline.BuildPlayer(scenes, buildFolder + "PCMaster/PrivateEyes.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Build / Client PC")]
    private static void BuildClientPC()
    {
        string[] scenes = CombineScenesAndAddFolder(clientScenes, sharedScenes);

        BuildPipeline.BuildPlayer(scenes, buildFolder + "PCClient/PrivateEyes.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Build / Client Android")]
    private static void BuildClientAndroid()
    {
        string[] scenes = CombineScenesAndAddFolder(clientScenes, sharedScenes);

        BuildPipeline.BuildPlayer(scenes, buildFolder + "AndroidClient/PrivateEyes.apk", BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Build / Client iOS")]
    private static void BuildClientiOS()
    {
        string[] scenes = CombineScenesAndAddFolder(clientScenes, sharedScenes);

        BuildPipeline.BuildPlayer(scenes, buildFolder + "iOSClient/PrivateEyes.ipa", BuildTarget.iOS, BuildOptions.None);
    }

    private static string[] CombineScenesAndAddFolder(string[] groupOne, string[] groupTwo)
    {
        int total = groupOne.Length + groupTwo.Length;
        string[] scenes = new string[total];

        for (int i = 0; i < total; i++)
        {
            if (i < groupOne.Length)
                scenes[i] = sceneFolder + groupOne[i];
            else
                scenes[i] = sceneFolder + groupTwo[i - groupOne.Length];
        }

        return scenes;
    }
}
