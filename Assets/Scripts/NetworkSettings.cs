using UnityEngine;
using System;

public class NetworkSettings : ScriptableObject
{
    [Serializable]
    public struct LocalConnectionSettings
    {
        public bool connectAutomatically;
        public string localIP;
    }

    public bool local;
    public LocalConnectionSettings localSettings;
    public string version = "0.1";

    public bool NeedIP
    {
        get
        {
            return local && !localSettings.connectAutomatically;
        }
    }
}
