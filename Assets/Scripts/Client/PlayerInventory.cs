using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInventory : Photon.MonoBehaviour{

    public Vector3 indicatorLocation;
    public GameObject itemIndicator;

    public Powerup powerup = null;

    public GameObject Key;

    //   private Item item;   
    //   private Knife knife;
    //    private Coroutine itemCoroutine;


    //Murders knife would not be an ItemInHand as of now.
 //   public ItemPickups ItemInHand = ItemPickups.Nothing;

    GameObject ItemInHand = null;

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
        print("remove item " + ItemInHand.GetComponent<Item>().ItemName);
        setIndicator(ItemInHand.GetComponent<Item>(), false);
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
      //      case "Evidence": ItemInHand = new Evidence(); break;
     //       case "Knife": ItemInHand = new Knife(); break;
            case "Key": ItemInHand = Key; break;

            default: ItemInHand = Key; break;
        }

        print("item in hand set to" + ItemInHand.GetComponent<Item>().ItemName);
        setIndicator(ItemInHand.GetComponent<Item>(), true);
    }

    void dropItem(Item itemToDrop)
    {
        if (photonView.isMine)
        {
            print("dropping item in hand" + itemToDrop.name);
            PhotonNetwork.Instantiate(itemToDrop.ResourceName, gameObject.transform.position, Quaternion.identity, 0);
        }

        setIndicator(itemToDrop, false);
    }

    void setIndicator(Item newItem, bool isVisible)
    {
        if (isVisible)
        {
            print("setting item indicator " + newItem.ItemName); 
            itemIndicator = Instantiate(Resources.Load(newItem.ResourceName, typeof(GameObject))) as GameObject;
            itemIndicator.GetComponent<BoxCollider>().enabled = false;
            itemIndicator.transform.position = gameObject.transform.position + indicatorLocation;
            itemIndicator.transform.parent = gameObject.transform;
        }
        else
        {
            print("destroying item indicator"); 
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
   //     if (itemCoroutine != null) StopCoroutine(itemCoroutine);

   //     ui.SetHeaderText("Pickup " + item.Name);
   //     itemCoroutine = StartCoroutine(ItemPickupCoroutine(item));

        Debug.Log("Encountered item." + item.GetComponent<Item>().ItemName);
    }

    IEnumerator ItemPickupCoroutine(Item item)
    {
        while (true)
        {
            // Detect input to pickup here
            yield return null;
        }
    }

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
