using UnityEngine;
using System.Collections;

// Base class for games across master and clients
// Game is responsible for state logic like switching between scenes/general game flow
[RequireComponent(typeof(PhotonView))]
public abstract class Game : Photon.MonoBehaviour
{
    // Common fields
    private GameNetwork networkField;
    protected GameNetwork network { get { return networkField; } }

    protected Lobby lobby;
    protected Session session;
    
    protected void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        networkField = gameObject.GetComponent<GameNetwork>();
    }

    // Override to provide the specific Session (ie Master/Client)
    protected abstract Session CreateSession(byte type);
        
    protected void InitiateMasterControl(int masterID)
    {
        // Assign our PhotonView over to the master
        photonView.TransferOwnership(masterID);
    }

    protected void LaunchSession(byte type)
    {
        lobby.Exit();
        session = CreateSession(type);
        session.Launch();
    }

    protected void FinishSession()
    {
        session.Finish();
        Destroy(session);
        session = null;
        lobby.Enter();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 200, 25), "Ping: " + PhotonNetwork.GetPing());
    }
}
