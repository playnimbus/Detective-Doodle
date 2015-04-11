using UnityEngine;
using System.Collections;


// Powerups that players can hold & use
public abstract class Powerup : MonoBehaviour
{
    // Use on yourself
    public abstract void Apply(Player player);

    #region Unity events

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
            coll.SendMessage("EncounteredPowerup", this, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
            coll.SendMessage("LeftPowerup", this, SendMessageOptions.DontRequireReceiver);
    }

    #endregion

}