using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : Photon.MonoBehaviour {

    public GameObject bystanderPlayer;
    public GameObject murdererPlayer;
    public GameObject vigilantePlayer;

    public GameObject connectBtn;
    public GameObject hostBtn;
    public Button attackBtn;

    bool isHost = false;

    private const string roomName = "RoomName";

	// Use this for initialization
	void Start () {
       PhotonNetwork.ConnectUsingSettings(".1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        GUI.Label(new Rect(10, 10, 230, 70), PhotonNetwork.countOfPlayers + "/8");
    }

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
        Debug.Log("Failed to Connect to Room");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");

        if (isHost == false)
        {
            GameObject tempPlayer = PhotonNetwork.Instantiate("bystanderPlayer", new Vector3(12, -16, 50), Quaternion.identity, 0);
            //GameObject tempCamera =  PhotonNetwork.Instantiate("bystanderCamera", new Vector3(12, -3.72F, 50), Quaternion.identity, 0);
            GameObject bystanderCamera = (GameObject)Instantiate(Resources.Load("bystanderCamera"));
            bystanderCamera.transform.position = tempPlayer.transform.position + new Vector3(0, 20, 0);

            tempPlayer.GetComponent<playerController>().playerCamera = bystanderCamera.GetComponent<Camera>();
            bystanderCamera.GetComponent<CameraFollow>().playerToFollow = tempPlayer.transform;
            tempPlayer.GetComponent<playerController>().playerThumbpad = bystanderCamera.GetComponent<CameraFollow>().playerThumbad;

        }
        else
        {
            GameObject hostCamera = (GameObject)Instantiate(Resources.Load("HostCamera"));
            hostCamera.transform.position = new Vector3(2.5f, 32.87f, 57.2f);
        }

        connectBtn.SetActive(false);
        hostBtn.SetActive(false);
    }
}
