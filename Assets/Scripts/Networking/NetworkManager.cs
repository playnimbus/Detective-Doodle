using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : Photon.MonoBehaviour {

    public GameObject bystanderPlayer;
    public GameObject murdererPlayer;
    public GameObject vigilantePlayer;

    public GameObject connectBtn;
    public GameObject hostBtn;
    public GameObject murdererWinsTxt;
    public GameObject murdererLosesTxt;

    public GameObject roomCovers;

    bool isHost = false;

    float murderAssignCountdown = 20;
    GameObject[] FinalPlayerList = null;
    public GameObject ChosenMurderer = null;
    bool murdererAssigned = false;
    TextMesh countDownText;


	// Use this for initialization
	void Start () {
       PhotonNetwork.ConnectUsingSettings(".1");
	}
	
	// Update is called once per frame
	void Update () {
	    if(isHost)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                PhotonNetwork.DestroyAll();
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
                PhotonNetwork.NetworkStatisticsReset();
                Application.LoadLevel(Application.loadedLevel);
            }

            if (PhotonNetwork.playerList.Length >= 4 && murdererAssigned == false)
            {
                murderAssignCountdown -= Time.deltaTime;

                countDownText.text = murderAssignCountdown.ToString("0.#");

                if (murderAssignCountdown <= 0)
                {
                    FinalPlayerList = GameObject.FindGameObjectsWithTag("Player");

                    ChosenMurderer = FinalPlayerList[Random.Range(0, FinalPlayerList.Length)];

                    GameObject clue = PhotonNetwork.Instantiate("Clue", ChosenMurderer.transform.position, Quaternion.identity, 0);
                    clue.transform.Rotate(new Vector3(90, 0, 0));

                    clue = PhotonNetwork.Instantiate("Clue", ChosenMurderer.transform.position, Quaternion.identity, 0);
                    clue.transform.Rotate(new Vector3(90, 0, 0));

                    clue = PhotonNetwork.Instantiate("Clue", ChosenMurderer.transform.position, Quaternion.identity, 0);
                    clue.transform.Rotate(new Vector3(90, 0, 0));

                    murdererAssigned = true;
                    countDownText.gameObject.SetActive(false);
                }
            }

            if (murdererAssigned == true)
            {
                if (ChosenMurderer == null)
                {
                    murdererLosesTxt.SetActive(true);
                }
            }
            if (murdererAssigned == true)
            {
                GameObject[] tempPlayerlist = GameObject.FindGameObjectsWithTag("Player");
                if (tempPlayerlist.Length <= 1)
                {
                    murdererWinsTxt.SetActive(true);
                }
            }
        }
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

        /*
        GUI.Label(new Rect(10, 70, 230, 70), "My Player ID: " + PhotonNetwork.player.ID);

        for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            GUI.Label(new Rect(10, 110 + (20 * i), 230, 70), "Player " + i + ": " + PhotonNetwork.playerList[i].ID);
        }
         * */
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
        Debug.Log("Failed to Connect");
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected");

        if (isHost == false)
        {
            //finds a random spawn position for the player
            GameObject[] playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
            Vector3 spawnPostition = playerSpawns[Random.Range(0,playerSpawns.Length)].transform.position;

            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            GameObject tempPlayer = PhotonNetwork.Instantiate("bystanderPlayer", spawnPostition, Quaternion.identity, 0);
            tempPlayer.name = "Player" + playerList.Length;
            //tempPlayer.GetComponentInChildren<SwordScript>().playerName = tempPlayer.name;

            GameObject bystanderCamera = (GameObject)Instantiate(Resources.Load("bystanderCamera"));

            bystanderCamera.transform.position = tempPlayer.transform.position + new Vector3(0, 20, 0);

            tempPlayer.GetComponent<playerController>().playerThumbpad = bystanderCamera.GetComponent<CameraFollow>().playerThumbad;
            tempPlayer.GetComponent<playerController>().playerCamera = bystanderCamera.GetComponent<Camera>();
            tempPlayer.GetComponent<playerController>().swordNotify = bystanderCamera.GetComponent<CameraFollow>().swordNotify;
            bystanderCamera.GetComponent<CameraFollow>().playerToFollow = tempPlayer.transform;
        }
        else
        {
            GameObject hostCamera = (GameObject)Instantiate(Resources.Load("HostCamera"));
            hostCamera.transform.position = new Vector3(2.5f, 32.87f, 57.2f);

            GameObject[] clueSpawns = GameObject.FindGameObjectsWithTag("WeaponSpawn");
            GameObject clue = PhotonNetwork.Instantiate("Clue", clueSpawns[Random.Range(0, clueSpawns.Length)].transform.position, Quaternion.identity, 0);
            clue.transform.Rotate(new Vector3(90, 0, 0));

     //       GameObject.Find("CreepSpawner").GetComponent<CreepSpawner>().spawnPhotonCreep();
     //       GameObject.Find("CreepSpawner").GetComponent<CreepSpawner>().spawnPhotonCreep();
     //       GameObject.Find("CreepSpawner").GetComponent<CreepSpawner>().spawnPhotonCreep();

            countDownText = GameObject.Find("CountDownTimer").GetComponent<TextMesh>();
            countDownText.text = murderAssignCountdown.ToString();

            roomCovers.SetActive(false);
        }

        connectBtn.SetActive(false);
        hostBtn.SetActive(false);
    }
}
