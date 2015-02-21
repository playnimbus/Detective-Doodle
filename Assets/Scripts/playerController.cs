using UnityEngine;
using System.Collections;

public class playerController : Photon.MonoBehaviour
{
    public Camera playerCamera;
    public Camera GuiCamera;

    public Animator playerAnimator;

    public GameObject playerThumbpad;
    public GameObject meleeSword;
    public GameObject swordSpawn;
    public GameObject swordNotify;
    public TextMesh name;

    public float cluesObtained;
    public float playerSpeed;

    public float currentVelocity;
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;

    private Vector3 thumbOrigin;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private bool hasWeapon;


    string[] names = { "Mike", "Nick", "Jacob", "John", "Joe", "Ian", "Marc","Swei","Flood","Mudry","Neo","Adam", "Yuka","Sarah","Brit","Casey","Jess","Nora","Jamie","Renzo","Keith","Kyle","Ryan","Kate","Ali","Rich", "Dave","Nate","Monty","Mina","Sam", "Erik" };

    void Start()
    {
        thumbOrigin = playerThumbpad.transform.localPosition;
        name.text = names[Random.Range(0, names.Length)];
        GuiCamera = GameObject.Find("bystanderGuiCamera").GetComponent<Camera>();

        if (PhotonNetwork.isMasterClient == true)
        {
            name.gameObject.SetActive(false);
        }

        
    }

    [RPC]
    void MakeMurderer()
    {
        print("I'm the murderer!");
    }

    // Update is called once per frame
    void Update()
    {

        currentVelocity = rigidbody.velocity.magnitude;

        animatePlayer();

        if (cluesObtained >= 3 && gameObject.name == "Player0")
        {
            hasWeapon = true;
            swordNotify.SetActive(true);
        }

        if (photonView.isMine)
        {
            MovementAnalog();
        }
        else if (!photonView.isMine)
        {
            Destroy(playerCamera);
            SyncedMovement(); 
        }
    }

    public void SwingSword() 
    {

        if (meleeSword.transform.position.y <= 25)
        {
            meleeSword.transform.position = new Vector3(0, 50, 0); 
        }
        else if (meleeSword.transform.position.y > 25)
        {
            meleeSword.transform.position = swordSpawn.transform.position;
        }

    } 

    void animatePlayer()
    {
        if (currentVelocity > 4)
        {
            //running
            playerAnimator.SetInteger("pose", 2);
        }
        else if (currentVelocity < 4 && currentVelocity > .35F)
        {
            //sneaking
            playerAnimator.SetInteger("pose", 1);
        }
        else
        {
            //idle
            playerAnimator.SetInteger("pose", 0);
        }

    }

    public void Attacked()
    {

    }

    IEnumerator MovementCoroutine()
    {
        Vector3 startPos = Input.mousePosition;

        while (Input.GetMouseButton(0))
        {
            Vector3 currentPos = Input.mousePosition;
            Vector3 delta = currentPos - startPos;

            delta.y = Mathf.Clamp(delta.y, -50, 50);
            delta.x = Mathf.Clamp(delta.x, -20, 20);
            
            gameObject.rigidbody.velocity = gameObject.transform.forward * (delta.y * 0.1f);
            gameObject.transform.localEulerAngles += new Vector3(0, delta.x * 0.1f, 0);

            yield return null;
        }

        rigidbody.velocity = Vector3.zero;
    }

    public void MovementAnalog()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = GuiCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "AnalogBG")
                    StartCoroutine(MovementCoroutine());
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(rigidbody.position);
        }
        else
        {
         //   print("stream is not writing");
            syncEndPosition = (Vector3)stream.ReceiveNext();
            syncStartPosition = rigidbody.position;

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;
        }
    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Clue")
        {
            print("found 1 clue!");
            GameObject.Destroy(collision.gameObject);
            cluesObtained += 1;


            GameObject[] clues = GameObject.FindGameObjectsWithTag("Clue");

            if (clues.Length <= 1)
            {
                GameObject[] clueSpawns = GameObject.FindGameObjectsWithTag("WeaponSpawn");
                GameObject clue = PhotonNetwork.Instantiate("Clue", clueSpawns[Random.Range(0, clueSpawns.Length)].transform.position, Quaternion.identity, 0);
                clue.transform.Rotate(new Vector3(90, 0, 0));
            }
            
        }
        else if (collision.collider.tag == "Sword")
        {
            Debug.Log(gameObject.name + "Such a sad day to stop living");
            
      //      PhotonNetwork.Disconnect();
     //       PhotonNetwork.NetworkStatisticsReset();
      //      PhotonNetwork.Destroy(gameObject);
      //      PhotonNetwork.LeaveRoom();

     //       if (PhotonNetwork.isMasterClient)
     //       {
                PhotonNetwork.Destroy(gameObject);
     //       }
     //       GameObject.Destroy(gameObject);
        }
    }


}