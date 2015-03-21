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
        if (collider.gameObject.CompareTag(Tags.Player)) collider.SendMessage("ApproachedStash", this, SendMessageOptions.DontRequireReceiver);
    }

    void TriggerExited(Collider collider)
    {
        if (collider.gameObject.CompareTag(Tags.Player)) collider.SendMessage("LeftStash", this, SendMessageOptions.DontRequireReceiver);
    }

    public void GetEvidence(Action<bool> callBack)
    {
        evidenceRequestCallback = callBack;   
        photonView.RPC("RequestEvidence", PhotonTargets.MasterClient);
    }

    [RPC]
    void RequestEvidence(PhotonMessageInfo info)
    {
        if(PhotonNetwork.isMasterClient)
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
