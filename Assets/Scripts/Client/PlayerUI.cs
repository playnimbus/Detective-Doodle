using UnityEngine;
using System.Collections;

// Displays what a player has gathered.
public class PlayerUI : MonoBehaviour 
{
    public GameObject[] evidence;
    private int next;

    void Start()
    {
        HideAllEvidence();
    }

    public void ShowNewEvidence()
    {
        if(next < evidence.Length)
        {
            evidence[next].SetActive(true);
            next++;
        }
    }

    public void HideAllEvidence()
    {
        next = 0;
        foreach (GameObject ev in evidence)
            ev.SetActive(false);
    }

}
