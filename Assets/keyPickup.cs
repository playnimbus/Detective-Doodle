using UnityEngine;
using System.Collections;

public class keyPickup : Photon.MonoBehaviour
{

    float activateTimer = 0.5f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        activateTimer -= Time.deltaTime;

    }

    void OnTriggerEnter(Collider collider)
    {
        if (activateTimer <= 0)
        {
            if (collider.gameObject.CompareTag(Tags.Player))
            {
                if (collider.gameObject.GetComponent<Player>().haveKey == false)
                {
                    collider.SendMessage("giveKey", this, SendMessageOptions.DontRequireReceiver);
                    photonView.RPC("destroyPickup", PhotonTargets.All);
                }

            }


        }
    }
    /*
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag(Tags.Player)) collider.SendMessage("LeftDoor", this, SendMessageOptions.DontRequireReceiver);
    }
     * */

    [RPC]
    void destroyPickup()
    {
        Destroy(gameObject);
    }
}
