using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInventory : Photon.MonoBehaviour{

    public GameObject evidenceIndictor;
    public GameObject keyIndictor;
    public Powerup powerup = null;

    //   private Item item;   
    //   private Knife knife;
    //    private Coroutine itemCoroutine;


    //Murders knife would not be an ItemInHand as of now.
    public ItemPickups ItemInHand = ItemPickups.Nothing;
    

    /// <summary>
    /// recieves an item and drops item in hand if player is carrying one
    /// </summary>
    /// <param name="newItem"></param>
    public void recieveItem(ItemPickups newItem)    
    {
        photonView.RPC("giveItem", PhotonTargets.All, (byte)newItem);
    }

    /// <summary>
    /// Removes item from player without dropping it on the floor.
    /// Mostly called if item has been used.
    /// </summary>
    public void removeItem()
    {
        photonView.RPC("removeItemRPC", PhotonTargets.All);
    }

    [RPC] 
    void removeItemRPC()
    {
     //   setIndicator(ItemInHand, false);

        evidenceIndictor.SetActive(false);
        keyIndictor.SetActive(false);

        ItemInHand = ItemPickups.Nothing;
    }


    [RPC] 
    void giveItem(byte newItem) //photon RPCs don't like enums apparently :/ but converting them to byte works when sending through RPC
    {
        if (ItemInHand != ItemPickups.Nothing)
        {
            dropItem(ItemInHand);
        }

        ItemInHand = (ItemPickups) newItem;
        setIndicator((ItemPickups)newItem, true);

        if (photonView.isMine && ItemInHand == ItemPickups.Evidence)
            gameObject.GetComponent<Player>().RecievedEvidence();
    }

    void dropItem(ItemPickups itemToDrop)
    {
        if (photonView.isMine)
        {
            switch (itemToDrop)
            {
                case ItemPickups.Key:
                    PhotonNetwork.Instantiate("keyPickup", gameObject.transform.position, Quaternion.identity, 0);
                    break;
                case ItemPickups.Evidence:
                    PhotonNetwork.Instantiate("evidencePickup", gameObject.transform.position + new Vector3(0, 23, 0), Quaternion.identity, 0);
                    gameObject.GetComponent<Player>().DroppedEvidence();
                    break;
            }
        }

        setIndicator(itemToDrop, false);
    }

    void setIndicator(ItemPickups itemToSet, bool isVisible)
    {
        switch (itemToSet)
        {
            case ItemPickups.Key: keyIndictor.SetActive(isVisible); break;
            case ItemPickups.Evidence: evidenceIndictor.SetActive(isVisible); break;
        }
    }



    public void EncounteredPowerup(Powerup powerup)
    {
        if (!photonView.isMine) return;

        this.powerup = powerup;
        gameObject.GetComponent<Player>().ui.SetPowerupIcon(powerup.Icon);
    }


        /*

    void EncounteredItem(Item item)
    {
        if (!photonView.isMine) return;
        
        if (itemCoroutine != null) StopCoroutine(itemCoroutine);

        ui.SetHeaderText("Pickup " + item.Name);
        itemCoroutine = StartCoroutine(ItemPickupCoroutine(item));

        Debug.Log("Encountered item.", item);
    }

    IEnumerator ItemPickupCoroutine(Item item)
    {
        while (true)
        {
            // Detect input to pickup here
            yield return null;
        }
    }

    void LeftItem(Item item)
    {
        if (!photonView.isMine) return;

        ui.SetHeaderText(string.Empty);
        if (itemCoroutine != null)
        {
            StopCoroutine(itemCoroutine);
            itemCoroutine = null;
        }

        Debug.Log("Left item.", item);
    }
    */

}
