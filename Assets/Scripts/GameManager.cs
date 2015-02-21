using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviour
{
    bool isHost = false;
    int playersJoined = 0;

	void Start ()
    {
        PhotonNetwork.ConnectUsingSettings(".1");
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

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("Failed to Connect");
    }

    // Called in master when room created
    void OnCreatedRoom()
    {
        PhotonNetwork.automaticallySyncScene = true;
    }

    // Called in client when room joined
    void OnJoinedRoom()
    {
        GetComponent<PhotonView>().RPC("PlayerJoined", 
                                        PhotonTargets.MasterClient, 
                                        new Object[] { });

        PhotonNetwork.automaticallySyncScene = true;
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
        playersJoined++;
      //  if (playersJoined > 1)
            StartCoroutine(WaitForStartCoroutine());
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

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
    }

}
