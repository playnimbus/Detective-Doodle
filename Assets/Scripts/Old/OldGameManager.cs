using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class OldGameManager : MonoBehaviour
{
    bool isHost = false;
    int playersJoined = 0;

	void Start ()
    {
        PhotonNetwork.ConnectUsingSettings(".1");
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
	}

    /*
     * Photon code
     */

    public void HostRoom()
    {
        PhotonNetwork.CreateRoom("DoodleDetective");
        isHost = true;
    }

    public void ConnectRoom()
    {
        PhotonNetwork.JoinRoom("DoodleDetective");
    }

    void OnPhotonCreateGameFailed()
    {
        Debug.Log("Failed to create room.");
    }

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("Failed to join room.");
    }

    // Called in master when room created
    void OnCreatedRoom()
    {
        Debug.Log("Room created.");
    }

    // Called in client when room joined
    void OnJoinedRoom()
    {
        Debug.Log("Room joined.");
        GetComponent<PhotonView>().RPC("PlayerJoined", 
                                        PhotonTargets.MasterClient, 
                                        null);
    }
    
    void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");   
    }

    IEnumerator WaitForStartCoroutine()
    {
        while(!Input.GetKeyDown(KeyCode.Space))
            yield return null;
              
        PhotonNetwork.LoadLevel("Main");
    }

    [RPC]
    public void PlayerJoined()
    {
        Debug.Log("RPC PlayerJoined() called.");
        playersJoined++;
        // if (playersJoined > 1)
            StartCoroutine(WaitForStartCoroutine());
    }
    
    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        
        GUI.Label(new Rect(10, 10, 230, 70), "Players Connected: " + PhotonNetwork.playerList.Length);
        GUI.Label(new Rect(10, 30, 230, 70), "Ping: " + PhotonNetwork.GetPing());
        GUI.Label(new Rect(10, 50, 230, 70), "isMasterClient: " + PhotonNetwork.isMasterClient);

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            GUI.Label(new Rect(10, 70 + (20 * i), 230, 70), "player list: " + PhotonNetwork.playerList[i].ID);
        }
    }

}
