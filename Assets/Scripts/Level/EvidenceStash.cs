using UnityEngine;
using System;

// Place where evidence will be 'hidden' in a room
public class EvidenceStash : Photon.MonoBehaviour 
{
    public Action<EvidenceStash> evidenceLooted;

    private Evidence evidence; // Unused for now
    private TriggerListener triggerListener;

    private Action<bool> evidenceRequestCallback;
    private bool hasEvidence;
    
    
    void Start()
    {
        hasEvidence = false;
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
        if (PhotonNetwork.isMasterClient)
        {
            Analytics.ObjectLooted(hasEvidence);
            photonView.RPC("ReceiveEvidence", info.sender, hasEvidence);
            if (hasEvidence && evidenceLooted != null) evidenceLooted(this);
            photonView.RPC("SetHasEvidence", PhotonTargets.All, false);
        }
    }

    [RPC]
    void ReceiveEvidence(bool hadEvidence)
    {
        hasEvidence = hadEvidence;
        evidenceRequestCallback(hasEvidence);
        evidenceRequestCallback = null;
    }

    [RPC]
    void SetHasEvidence(bool value)
    {
        hasEvidence = value;
    }

    public bool getHasEvidence()
    {
        return hasEvidence;
    }
}
