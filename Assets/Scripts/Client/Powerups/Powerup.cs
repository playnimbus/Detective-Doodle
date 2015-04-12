using UnityEngine;
using System.Collections;


// Powerups that players can hold & use
public abstract class Powerup : MonoBehaviour
{
    // Use on yourself
    public abstract bool Apply(Player player);      //activates powerup and applys effect to player
    public Sprite Icon;

    #region Unity events

    void OnTriggerEnter(Collider coll)      //equips powerup to players slot
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            coll.SendMessage("EncounteredPowerup", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            coll.SendMessage("LeftPowerup", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion

}