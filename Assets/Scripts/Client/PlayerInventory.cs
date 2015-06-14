using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInventory : Photon.MonoBehaviour{

    public Vector3 indicatorLocation;
    public GameObject itemIndicator;

    public Powerup powerup = null;

    public GameObject Key;
    public GameObject Evidence;

    //   private Item item;   
    //   private Knife knife;
    //    private Coroutine itemCoroutine;


    //Murders knife would not be an ItemInHand as of now.
 //   public ItemPickups ItemInHand = ItemPickups.Nothing;

    public GameObject ItemInHand = null;

    /// <summary>
    /// recieves an item and drops item in hand if player is carrying one
    /// </summary>
    /// <param name="newItem"></param>
    public void recieveItem(GameObject newItem)    
    {
        photonView.RPC("giveItem", PhotonTargets.All, newItem.GetComponent<Item>().ItemName);
    }

    public void recieveItem(string newItem)
    {
        photonView.RPC("giveItem", PhotonTargets.All, newItem);
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
        setIndicator(ItemInHand.GetComponent<Item>(), false);
        ItemInHand = null;
    }


    [RPC] 
    void giveItem(string newItemName)  //Can't send GameObjects over RPC :(
    {

        if (ItemInHand != null)
        {
            print("Item inhand before drop " + ItemInHand.GetComponent<Item>().ItemName);
            dropItem(ItemInHand.GetComponent<Item>());
        }

        switch (newItemName)
        {
            case "Evidence": ItemInHand = Evidence; break;
     //       case "Knife": ItemInHand = new Knife(); break;
            case "Key": ItemInHand = Key; break;

            default: ItemInHand = Key; break;
        }
        setIndicator(ItemInHand.GetComponent<Item>(), true);
    }

    void dropItem(Item itemToDrop)
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Instantiate(itemToDrop.ResourceName, gameObject.transform.position, Quaternion.identity, 0);
        }
        setIndicator(itemToDrop, false);
    }

    void setIndicator(Item newItem, bool isVisible)
    {
        if (isVisible)
        {
            itemIndicator = Instantiate(Resources.Load(newItem.ResourceName, typeof(GameObject))) as GameObject;
            itemIndicator.GetComponent<BoxCollider>().enabled = false;
            itemIndicator.transform.parent = gameObject.GetComponentInChildren<PlayerModel>().transform;
            itemIndicator.transform.localPosition = itemIndicator.GetComponent<Item>().LocalPositionOnPlayer;
            itemIndicator.transform.localRotation = Quaternion.LookRotation(itemIndicator.GetComponent<Item>().LocalRotationOnPlayer);
        }
        else
        {
            GameObject.Destroy(itemIndicator);
        }
    }

    public void EncounteredPowerup(Powerup powerup)
    {
        if (!photonView.isMine) return;

        this.powerup = powerup;
        gameObject.GetComponent<Player>().ui.SetPowerupIcon(powerup.Icon);
    }


    void EncounteredItem(GameObject item)
    {
        if (!photonView.isMine) return;

        recieveItem(item);
    }


    //not used right now cause items get automatically picked up
    //This is useful if items require an action to be picked up
    void LeftItem(GameObject item)
    {
        if (!photonView.isMine) return;

  //      ui.SetHeaderText(string.Empty);
 //       if (itemCoroutine != null)
  //      {
  //          StopCoroutine(itemCoroutine);
   //         itemCoroutine = null;
  //      }

        Debug.Log("Left item." + item.GetComponent<Item>().ItemName);
    }
    

}
