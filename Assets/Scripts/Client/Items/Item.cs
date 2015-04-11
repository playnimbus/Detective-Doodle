using UnityEngine;
using System.Collections;

// Items that players can pick up and use on other players
public abstract class Item : MonoBehaviour
{
    // Use on other players >:)
    public abstract void Use(Player target);

    // Name of the object
    public abstract string Name { get; }

    #region Unity events

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player)) 
            coll.SendMessage("EncounteredItem", this, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player)) 
            coll.SendMessage("LeftItem", this, SendMessageOptions.DontRequireReceiver);
    }

    #endregion
}
