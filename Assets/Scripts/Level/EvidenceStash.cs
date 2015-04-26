using UnityEngine;
using System.Collections;
using System;

// Place where evidence will be 'hidden' in a room
public class EvidenceStash : Photon.MonoBehaviour 
{
    public Action<EvidenceStash> evidenceLooted;

    private Evidence evidence; // Unused for now
    private TriggerListener triggerListener;

    private Action<bool> evidenceRequestCallback;
    public bool hasEvidence;
    public bool hasSpeedBoost;
    public bool isBeingLootedByPlayer;

    //should switch loot type over to an enum assuming stashes will only have one type of loot at a time
 //   enum TypeOfLoot {None, Evidence, SpeedPowerUp, Money};
 //   private TypeOfLoot hasLoot = TypeOfLoot.None;

    public bool HasEvidence{
        get { return hasEvidence; }
    }
    public bool HasSpeedBoost{
        get { return hasSpeedBoost; }
    }
    public bool IsBeingLootedByPlayer
    {
        get { return isBeingLootedByPlayer; }
    }
    
    void Start()
    {
        hasEvidence = false;
        hasSpeedBoost = false;
        isBeingLootedByPlayer = false;

        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(RestockLoot(1));
        }

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

    public void GetEvidence()//Action<bool> callBack)
    {
       // evidenceRequestCallback = callBack;   
        photonView.RPC("RequestEvidence", PhotonTargets.MasterClient);
    }

    public void LockStashFromOthers(bool isLocked)
    {
        photonView.RPC("setIsBeingLootedByPlayer", PhotonTargets.All, isLocked);
    }

    [RPC]
    void RequestEvidence(PhotonMessageInfo info)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Analytics.ObjectLooted(hasEvidence);

          //  photonView.RPC("ReceiveEvidence", info.sender, hasEvidence);
            if (hasEvidence && evidenceLooted != null) evidenceLooted(this);
            photonView.RPC("SetHasEvidence", PhotonTargets.All, false);
            photonView.RPC("SetHasSpeedBoost", PhotonTargets.All, false);

            StartCoroutine(RestockLoot(UnityEngine.Random.Range(20, 40)));
        }
    }
    
    IEnumerator RestockLoot(float time)
    {
        yield return new WaitForSeconds(time);

        int randomNum = UnityEngine.Random.Range(0, 10);

        if (randomNum == 2)
        {
            print("stash given Evidence");
            photonView.RPC("SetHasEvidence", PhotonTargets.All, true);
        }
        else if(randomNum >= 8){
            print("stash given speedBoost");
            photonView.RPC("SetHasSpeedBoost", PhotonTargets.All, true);
        }
        else
        {
            print("stash not given loot");
            StartCoroutine(RestockLoot(UnityEngine.Random.Range(20, 40)));
        }
    }
/*
    [RPC]
    void ReceiveEvidence(bool hadEvidence)
    {
        hasEvidence = hadEvidence;
        evidenceRequestCallback(hasEvidence);
        evidenceRequestCallback = null;
    }
*/
    [RPC]
    void SetHasEvidence(bool value)
    {
        hasEvidence = value;
    }

    [RPC]
    void SetHasSpeedBoost(bool value)
    {
        hasSpeedBoost = value;
    }

    [RPC]
    void setIsBeingLootedByPlayer(bool value)
    {
        isBeingLootedByPlayer = value;
    }
}
