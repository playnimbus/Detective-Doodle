using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Player : Photon.MonoBehaviour
{
    public GameObject evidenceIndictor;
    public GameObject keyIndictor;
    public GameObject bunnyModel;
    public GameObject wolfModel;
    public Text name;
    LootMicroGame LootDrawer;

    public static class PlayerAction
    { 
        public const byte MurdererAccused = 0;
        public const byte PlayerKilled = 1;
        public const byte PlayerAccused = 2;
    }
    public Action<byte> action;

    public bool IsMurderer { get; private set; }
    public bool IsDetective { get; private set; }
    public bool IsDead { get; private set; }

    private new PlayerCamera camera;
    private PlayerUI ui;
    private Coroutine stashSearch;
    private AudioBank audio;
    private bool haveEvidence;
    private bool haveKey;
    private bool canMurder;

    public AudioBank Audio { set { audio = value; } }
    public Player otherPlayer;

    // New variables being added to switch over to a item-powerup scheme
    private Item item;
    public Powerup powerup = null;
    private Knife knife;
    private Coroutine itemCoroutine;
    private Coroutine powerupCoroutine;

    #region Initialization

    // Acts as a Start() for network instantiated objects
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        InitUI();
        InitCamera();
        SetHaveEvidence(false);

        MakeDetective();        //used for no detective game. Make everyone detective
                                //comment out to re-enable detectives
                                //also uncomment code from whoDunnitMasterSession

        // HACK ... Kinda. Assumes we only have one text field on the player
        GetComponentInChildren<Text>().text = photonView.owner.name;

        if (photonView.isMine)
        {
            if (UnityEngine.Random.Range(0, 2) < 1)
            {
                photonView.RPC("SetPlayerModel", PhotonTargets.All, "bunny");
            }
            else
            {
                photonView.RPC("SetPlayerModel", PhotonTargets.All, "wolf");
            }
        }
    }

    void InitUI()
    {
        if (!photonView.isMine) return;
        
        GameObject menuGO = Instantiate(Resources.Load<GameObject>("ClientMenu")) as GameObject;
        ui = menuGO.GetComponent<PlayerUI>();
        ui.SetHeaderText("No Evidence");

        GameObject LootGame = Instantiate(Resources.Load<GameObject>("LootGame")) as GameObject;
        LootGame.transform.position = new Vector3(-100, 0, 0);
        LootDrawer = LootGame.GetComponent<LootMicroGame>();

        ui.InitPowerupButton( () =>     //called when powerup button is pressed
        {
            if(powerup!=null)
            {
                if (powerup.Apply(this))    //reutrns false if powerup does not execute     ex: if you try to uniform swap and no one is in range
                {
                    powerup = null;
                    ui.SetPowerupIcon(null);
                }
            }
        });
    }
    
    void InitCamera()
    {
        camera = GetComponentInChildren<PlayerCamera>();
        if (camera == null) Debug.LogError("[Player] Couldn't find PlayerCamera", this);

        if (photonView.isMine)
        {
            camera.Init(this.transform);
            camera.transform.parent = null;
        }
        else Destroy(camera.gameObject);
    }

    [RPC]
    public void SetPlayerModel(string modelName)
    {
        if (modelName == "bunny")
        {
            bunnyModel.SetActive(true);
        }
        else if (modelName == "wolf")
        {
            wolfModel.SetActive(true);
        }
    }

    [RPC]
    public void MakeMurderer()
    {
        IsMurderer = true;
        IsDetective = false;
        if (photonView.isMine)
        {
            ui.MarkAsMurderer();
            canMurder = true;
        }
    }

    [RPC]
    public void MakeDetective()
    {
        IsDetective = true;
     //   GetComponent<Renderer>().material.color = Color.blue;
        if (photonView.isMine)
            ui.MarkAsDetective(true);
    }

    [RPC]
    public void RemoveDetectiveship()
    {
        IsDetective = false;
        if (photonView.isMine)
            ui.MarkAsDetective(false);
    }

    #endregion

    #region Environment Interaction

    public void ApproachedStash(EvidenceStash stash)
    {
        if (!photonView.isMine) return;

        LootDrawer.MakeDrawerAvailable(stash, this);
    }

    public void LeftStash(EvidenceStash stash)
    {
        if (!photonView.isMine) return;

        LootDrawer.MakeDrawerHidden();

    }

    public void giveEvidence()
    {
        photonView.RPC("SetHaveEvidence", PhotonTargets.All, true);
    }
    public void giveKey()
    {
        photonView.RPC("SetHaveKey", PhotonTargets.All, true);
    }
        
    void EnteredRoom(LevelRoom room)
    {
        if (!photonView.isMine) return;

        // camera.MoveToVantage(room.overheadCameraPosition);
        room.Reveal();
    }

    void ExitedRoom(LevelRoom room)
    {
        if (!photonView.isMine) return;

        // camera.ResumeFollow();
        room.Conceal();
    }

    void EncounteredItem(Item item)
    {
        if (!photonView.isMine) return;
        
        if (itemCoroutine != null) StopCoroutine(itemCoroutine);

        ui.SetHeaderText("Pickup " + item.Name);
        itemCoroutine = StartCoroutine(ItemPickupCoroutine(item));

        Debug.Log("Encountered item.", item);
    }

    IEnumerator ItemPickupCoroutine(Item item)
    {
        while (true)
        {
            // Detect input to pickup here
            yield return null;
        }
    }

    void LeftItem(Item item)
    {
        if (!photonView.isMine) return;

        ui.SetHeaderText(string.Empty);
        if (itemCoroutine != null)
        {
            StopCoroutine(itemCoroutine);
            itemCoroutine = null;
        }

        Debug.Log("Left item.", item);
    }

    public void EncounteredPowerup(Powerup powerup)
    {
        if (!photonView.isMine) return;

        this.powerup = powerup;
        ui.SetPowerupIcon(powerup.Icon);
    }

    #endregion

    #region Player-Player Interaction

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.isMine) return;
        otherPlayer = collision.gameObject.GetComponent<Player>();
        if (otherPlayer == null) return;
        if (IsDead) return;
        
        // return when you do an action!
        if (IsMurderer)
        {
            if (MurdererInteraction()) return;
        }
        else if (IsDetective)
        {
            if (DetectiveInteraction()) return;
        }
        else // Is bystander
        {
            if (BystanderInteraction()) return;
        }

        // All players can loot dead 
        if (LootingInteraction()) return;
    }

    bool MurdererInteraction()
    {
        int murderButton = 0;
        bool interacted = false;

        // If it's the detective and we have evidence, we can give it to him.
        /*
        if (otherPlayer.IsDetective && haveEvidence)
        {
            murderButton = 1; // This changes which button we use to murder though
            ui.ShowButton(0, "Give Evidence", true, () =>
            {
                otherPlayer.photonView.RPC("SetHaveEvidence", PhotonTargets.All, true);
                photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
                ui.HideAllButtons();
            });
            interacted = true;
        }
        */

        // Now our murder business
        if (canMurder)
        {
            ui.ShowButton(murderButton, "Murder", true, () =>
            {
                otherPlayer.photonView.RPC("Kill", PhotonTargets.All);
                canMurder = false;
                Invoke("ResetCanMurder", 15f);
                ui.FadeInMurderIcon(15f);
                ui.HideAllButtons();
            });
            interacted = true;
        }

        return interacted;
    }

    bool DetectiveInteraction()
    {
        if (haveEvidence)
        {
            ui.ShowButton(0, "Accuse", true, () =>
            {
                otherPlayer.photonView.RPC("Accuse", PhotonTargets.All);
                photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
                
                // If we're wrong we also are punished!
                if (!otherPlayer.IsMurderer) photonView.RPC("Accuse", PhotonTargets.All);

                Analytics.PlayerAccused(otherPlayer.IsMurderer);
            });
            return true;
        }

        return false;
    }

    bool BystanderInteraction()
    {
        if (otherPlayer.IsDetective && haveEvidence && !otherPlayer.haveEvidence && !otherPlayer.IsDead)
        {
            ui.ShowButton(0, "Give Evidence", true, () =>
            {
                otherPlayer.photonView.RPC("SetHaveEvidence", PhotonTargets.All, true);
                photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
            });
            return true;
        }
        
        if (otherPlayer.IsDetective && otherPlayer.IsDead)
        {
            ui.ShowButton(0, "Become Detective", true, () =>
                {
                    otherPlayer.photonView.RPC("RemoveDetectiveship", PhotonTargets.All);
                    photonView.RPC("MakeDetective", PhotonTargets.All);
                });
            return true;
        }
        
        return false;
    }

    bool LootingInteraction()
    {
        if (otherPlayer.IsDead && otherPlayer.haveEvidence)
        {
            ui.ShowButton(0, "Take Evidence", true, () =>
            {
                otherPlayer.photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
                photonView.RPC("SetHaveEvidence", PhotonTargets.All, true);
            });
            return true;
        }
        return false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (!photonView.isMine) return;
        Player other = collision.gameObject.GetComponent<Player>();
        if(other != null)
        {
            ui.HideAllButtons();
        }
    }

    #endregion

    #region State Functions

    [RPC]
    void Accuse()
    {
        if (IsMurderer)
        {
            GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            GetComponent<PlayerMovement>().StopMovement(0f); // 0 = indefinitely
            if (action != null)
            {
                action(PlayerAction.MurdererAccused);
            }
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.yellow;
            GetComponent<PlayerMovement>().StopMovement(22f);
            Invoke("ResetColor", 22f);
            if (action != null)
            {
                action(PlayerAction.PlayerAccused);
            }
        }
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = IsDetective ? Color.blue : Color.white;
    }

    void ResetCanMurder()
    {
        canMurder = true;
    }
    
    [RPC]
    void Kill()
    {
        if (!IsDead)
        {
            IsDead = true;
            GetComponent<Renderer>().material.color = Color.red;
            GetComponent<PlayerMovement>().StopMovement(0f);
            CancelInvoke("ResetColor");
            if (photonView.isMine && ui != null)
            {
                ui.SetHeaderText("You have been murdered!");
            }
            if (action != null)
            {
                action(PlayerAction.PlayerKilled);
            }
        }
    }

    [RPC]
    void SetHaveEvidence(bool value)
    {

        haveEvidence = value;
        evidenceIndictor.SetActive(value);

        haveKey = false;
        keyIndictor.SetActive(false);

/*
        if (photonView.isMine && ui != null)
        {
            string[] evidence = { "Hammer", "Knife", "Lead Pipe", "Revolver", "Rope", "Wrench" };
            ui.SetHeaderText(value ? "Evidence: " + evidence[UnityEngine.Random.Range(0, evidence.Length)] : "No Evidence");
        }
 * */
    }

    [RPC]
    void SetHaveKey(bool value)
    {
        haveKey = value;
        keyIndictor.SetActive(value);

        haveEvidence = false;
        evidenceIndictor.SetActive(false);
    }

    #endregion

    public void BystandersWon()
    {
        if (IsMurderer)
        {
            ui.SetHeaderText("You have been caught!");
        }
        else
        {
            ui.SetHeaderText("You have won!");
        }
    }

    public void MurdererWon()
    {
        if (IsMurderer)
        {
            ui.SetHeaderText("You have won!");
        }
        else
        {
            ui.SetHeaderText("Everyone has been murdered!");
        }
    }

}