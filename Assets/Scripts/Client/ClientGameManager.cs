using UnityEngine;
using System.Collections;

// Client specific game state management logic
public class ClientGameManager : GameManager
{
    private string message = "Client";

    protected override void Start()
    {
        base.Start();
        networkManager.onConnected += networkManager.JoinRoom;
        networkManager.onJoinedRoom += JoinedRoom;
        networkManager.onJoinRoomFailed += networkManager.JoinRoom;
        networkManager.onGameCommunicatorCreated += GameCommunicatorCreated;
    }

    void JoinedRoom()
    {

    }

    void GameCommunicatorCreated(int photonID)
    {
        communicator = gameObject.AddComponent<GameCommunicator>();
        communicator.photonView.viewID = photonID;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 230, 70), "Message: " + message);
    }
}
