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
                if (collider.gameObject.GetComponent<PlayerInventory>().ItemInHand != ItemPickups.Key)
                {
                    collider.gameObject.GetComponent<PlayerInventory>().recieveItem(ItemPickups.Key);
                    photonView.RPC("destroyPickup", PhotonTargets.All);
                }
            }
        }
    }

    [RPC]
    void destroyPickup()
    {
        Destroy(gameObject);
    }
}
