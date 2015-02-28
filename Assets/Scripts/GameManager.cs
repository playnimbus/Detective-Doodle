using UnityEngine;
using System.Collections;

// Base class for game manager across master and clients
// Game manager is responsible for game state logic like switching between scenes
// and communictions across clients
public class GameManager : MonoBehaviour 
{
    private NetworkManager networkManagerField;
    protected NetworkManager networkManager { get { return networkManagerField; } }

    protected GameCommunicator communicator;

    protected virtual void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        networkManagerField = gameObject.AddComponent<NetworkManager>();
    }
}
