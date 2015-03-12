using UnityEngine;
using System;

// Place where evidence will be 'hidden' in a room
public class EvidenceStash : MonoBehaviour 
{
    private Evidence evidence;
    private TriggerListener triggerListener;

    // Called when a player comes near or leaves the stash
    public Action<EvidenceStash, Player> onPlayerApproach;
    public Action<EvidenceStash, Player> onPlayerLeave;

    void Start()
    {
        triggerListener = GetComponent<TriggerListener>();
        triggerListener.onTriggerEntered += TriggerEntered;
        triggerListener.onTriggerExited += TriggerExited;
    }

    void TriggerEntered(Collider collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null && onPlayerApproach != null) onPlayerApproach(this, p);
    }

    void TriggerExited(Collider collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null && onPlayerLeave != null) onPlayerLeave(this, p);
    }
}
