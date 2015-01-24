using UnityEngine;
using System.Collections;

public class playerController : Photon.MonoBehaviour
{
    public GameObject playerCursor;
    public Camera playerCamera;
    public float playerSpeed;

    public GameObject meleeGO;
    float meleeTimer = 0;
    float meleeUpTime = 0.2f;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            MovementAnalog();

            if (meleeGO.activeInHierarchy == true)
            {
                meleeTimer += Time.deltaTime;
                if (meleeTimer >= meleeUpTime)
                {
                    meleeGO.SetActive(false);
                    meleeTimer = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!photonView.isMine)
        {
            SyncedMovement();
        }
    }

    public void SwingSword()
    {
        meleeGO.SetActive(true);
    }

    public void MovementAnalog()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "AnalogStick")
                {
                    Vector3 normalizedCastPosition = hit.point - hit.transform.position;
                    Vector3 forceToAdd = new Vector3(((hit.point.x - hit.transform.position.x) * playerSpeed), 0, ((hit.point.z - hit.transform.position.z) * playerSpeed));
                    //gameObject.rigidbody.AddForce(forceToAdd);
                    gameObject.rigidbody.velocity = forceToAdd;
                    transform.LookAt(gameObject.transform.position + forceToAdd);
                }

                if (hit.transform.gameObject.name == "AttackButton")
                {
                    SwingSword();
                }
            }
            else
            {
                gameObject.rigidbody.velocity = Vector2.zero;

            }
        }
        else
        {
            gameObject.rigidbody.velocity = Vector2.zero;

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
}