using UnityEngine;
using System;

// Place where evidence will be 'hidden' in a room
public class EvidenceStash : Photon.MonoBehaviour 
{
    private Evidence evidence; // Unused for now
    private TriggerListener triggerListener;

    // Called when a player comes near or leaves the stash
    public Action<EvidenceStash, Player> onPlayerApproach;
    public Action<EvidenceStash, Player> onPlayerLeave;

    private Action<bool> evidenceRequestCallback;
    private bool hasEvidence;
    
    void Start()
    {
        hasEvidence = true;
        triggerListener = GetComponent<TriggerListener>();
        triggerListener.onTriggerEntered += TriggerEntered;
        triggerListener.onTriggerExited += TriggerExited;
    }

    void TriggerEntered(Collider collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null) p.gameObject.SendMessage("ApproachedStash", this, SendMessageOptions.DontRequireReceiver);
    }

    void TriggerExited(Collider collider)
    {
        Player p = collider.GetComponent<Player>();
        if (p != null) p.gameObject.SendMessage("LeftStash", this, SendMessageOptions.DontRequireReceiver);
    }

    public void GetEvidence(Action<bool> callBack)
    {
        evidenceRequestCallback = callBack;   
        photonView.RPC("RequestEvidence", PhotonTargets.MasterClient);
    }

    [RPC]
    void RequestEvidence(PhotonMessageInfo info)
    {
        // Should only be true for master as this is a scene object
        if(photonView.isMine)
        {
            photonView.RPC("ReceiveEvidence", info.sender, hasEvidence);
            hasEvidence = false;
        }
    }

    [RPC]
    void ReceiveEvidence(bool hadEvidence)
    {
        hasEvidence = hadEvidence;
        evidenceRequestCallback(hasEvidence);
        evidenceRequestCallback = null;
    }

}
