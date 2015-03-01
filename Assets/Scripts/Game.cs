using UnityEngine;
using System.Collections;

// Base class for games across master and clients
// Game is responsible for state logic like switching between scenes/general game flow
[RequireComponent(typeof(PhotonView))]
public class Game : Photon.MonoBehaviour
{
    // Common fields
    private GameNetwork networkField;
    protected GameNetwork network { get { return networkField; } }
    
    protected string message;

    protected virtual void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        networkField = gameObject.AddComponent<GameNetwork>();
    }

    /*void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 230, 70), "Message: " + message);
    }*/
}
