using UnityEngine;
using System.Collections;

// Items that players can pick up and use on other players
public abstract class Item : Photon.MonoBehaviour
{
    // Use on other players >:)
    public abstract void Use(Player target);

    // Name of the object
    public abstract string ItemName { get; }
    public abstract string ResourceName { get; }
    public abstract Vector3 LocalPositionOnPlayer { get; }
    public abstract Vector3 LocalRotationOnPlayer { get; }

    #region Unity events

    float activateTimer = 0.5f;

    void Update()
    {
        activateTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (activateTimer <= 0)
        {
            if (coll.gameObject.CompareTag(Tags.Player))
            {
                print("item entered");
                coll.SendMessage("EncounteredItem", gameObject, SendMessageOptions.DontRequireReceiver);
                if (GetComponent<PhotonView>().isMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            print("item left");
            coll.SendMessage("LeftItem", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
    #endregion
}
