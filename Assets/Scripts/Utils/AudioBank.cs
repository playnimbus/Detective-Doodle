using UnityEngine;
using System.Collections.Generic;

public class AudioBank : MonoBehaviour 
{
    public AudioClip[] clips;
    private Dictionary<string, AudioSource> sources;

    void Awake()
    {
        TryInit();   
    }

    void TryInit()
    {
        if (sources != null) return;

        sources = new Dictionary<string, AudioSource>(clips.Length * 2);
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
            {
                Debug.LogWarning("[SoundBank] Clip[" + i + "] is empty!", this);
                continue;
            }
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = clips[i];            
            sources[clips[i].name] = source;
        }
    }

    public void PlaySound(string name, bool loop = false)
    {
        TryInit();

        AudioSource source;
        if (sources.TryGetValue(name, out source))
        {
            source.loop = loop;
            source.Play();
        }
        else
        {
            Debug.Log("[SoundBank] " + name + " not found!", this);
        }
    }
}
