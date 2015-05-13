using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Player : Photon.MonoBehaviour
{

    public GameObject falseAccusationIndicator;
    public GameObject bunnyModel;
    public GameObject wolfModel;
    public Text name;

    LootMicroGame LootDrawer;
    LockedDoorUI lockedDoorUI;

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
    public PlayerUI ui;
    public PlayerInventory inventory;
    private Coroutine stashSearch;
    private AudioBank audio;
    public TriggerListener interactionTrigger;

    private bool canMurder;

    public AudioBank Audio { set { audio = value; } }

    [HideInInspector]
    public Player interactionTarget;

    #region Initialization

    // Acts as a Start() for network instantiated objects
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        InitUI();
        InitCamera();
        MakeDetective();        //used for no detective game. Make everyone detective
                                //comment out to re-enable detectives
                                //also uncomment code from whoDunnitMasterSession

        inventory = gameObject.GetComponent<PlayerInventory>();

        // HACK ... Kinda. Assumes we only have one text field on the player
        GetComponentInChildren<Text>().text = photonView.owner.name;

        // Listen to trigger for player-player interactions
        interactionTrigger.onTriggerEntered += OnInteractionEnter;
        interactionTrigger.onTriggerExited += OnInteractionExit;

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

            Destroy(GameObject.Find("Main Camera"));
            Destroy(GameObject.Find("Main Camera 1"));
        }
    }

    void InitUI()
    {
        if (!photonView.isMine) return;
        
        GameObject menuGO = Instantiate(Resources.Load<GameObject>("ClientMenu")) as GameObject;
        ui = menuGO.GetComponent<PlayerUI>();

        GameObject LootGame = Instantiate(Resources.Load<GameObject>("LootGame")) as GameObject;
        LootGame.transform.position = new Vector3(-100, 0, 0);

        LootDrawer = LootGame.GetComponent<LootMicroGame>();
        lockedDoorUI = LootGame.GetComponentInChildren<LockedDoorUI>();

        ui.InitPowerupButton(() =>     //called when powerup button is pressed
        {
            Powerup powerup = gameObject.GetComponent<PlayerInventory>().powerup;
            if (powerup != null)
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
       // if (photonView.isMine)
            //ui.MarkAsDetective(true);
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

    public void ApproachedDoor(Door door)
    {
        if (!photonView.isMine) return;

        lockedDoorUI.MakeDoorAvailable(door, this);
    }

    public void LeftDoor(Door door)
    {
        if (!photonView.isMine) return;

        lockedDoorUI.MakeDoorHidden();
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

    #endregion

    #region Player-Player Interaction

    void OnInteractionEnter(Collider other)
    {
        if (!photonView.isMine) return;
        Player otherPlayer = other.gameObject.GetComponent<Player>();
        if (otherPlayer == null) return;
        if (IsDead) return;

        interactionTarget = otherPlayer;

        // return when you do an action!
        if (IsMurderer)
        {
            if (MurdererInteraction(otherPlayer)) return;
        }
        else if (IsDetective)
        {
            if (DetectiveInteraction(otherPlayer)) return;
        }
        else // Is bystander
        {
            if (BystanderInteraction(otherPlayer)) return;
        }

        // All players can loot dead 
        if (LootingInteraction(otherPlayer)) return;
    }


    bool MurdererInteraction(Player otherPlayer)
    {
        bool interacted = false;

        // Now our murder business
        if (canMurder)
        {
            ui.ShowButton(1, "Murder", true, () =>
            {
                otherPlayer.photonView.RPC("Kill", PhotonTargets.All);
                canMurder = false;
                Invoke("ResetCanMurder", 15f);
                ui.FadeInMurderIcon(15f);
            });
            interacted = true;

            ui.ShowButton(0, "Shove", true, () =>
            {
                otherPlayer.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });
        }
        else
        {
            ui.SetTapAction("Shove", () =>
            {
                otherPlayer.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });
        }

        return interacted;
    }

    bool DetectiveInteraction(Player otherPlayer)
    {
        if (inventory.ItemInHand == ItemPickups.Evidence)
        {
            ui.ShowButton(0, "Accuse", true, () =>
            {
                otherPlayer.photonView.RPC("Accuse", PhotonTargets.All);
                inventory.removeItem();

                // If we're wrong we also are punished!
                if (!otherPlayer.IsMurderer) photonView.RPC("Accuse", PhotonTargets.All);

                Analytics.PlayerAccused(otherPlayer.IsMurderer);
            });

            ui.ShowButton(1, "Shove", true, () =>
            {
                otherPlayer.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });

            return true;
        }
        else
        {
            ui.SetTapAction("Shove", () =>
            {
                otherPlayer.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });
        }

        return false;
    }

    // Not used currently
    bool BystanderInteraction(Player otherPlayer)
    {
        if (otherPlayer.IsDetective && !otherPlayer.IsDead)
        {
            ui.ShowButton(0, "Give Evidence", true, () =>
            {
                otherPlayer.inventory.recieveItem(ItemPickups.Evidence);
                inventory.removeItem();
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

    bool LootingInteraction(Player otherPlayer)
    {
        if (otherPlayer.IsDead && otherPlayer.inventory.ItemInHand == ItemPickups.Evidence)
        {
            ui.SetTapAction("Take Evidence", () =>
            {
                otherPlayer.inventory.removeItem();
                inventory.recieveItem(ItemPickups.Evidence);
            });
            return true;
        }
        return false;
    }

    void OnInteractionExit(Collider other)
    {
        if (!photonView.isMine) return;
        Player otherPlayer = other.gameObject.GetComponent<Player>();
        if (otherPlayer != null)
        {
            ui.RemoveTapAction();
            ui.HideAllButtons();
            interactionTarget = null;
        }
    }

    #endregion

    #region State Functions

    [RPC]
    void Accuse()
    {
        if (IsMurderer)
        {
           // GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f);
            GetComponent<PlayerMovement>().StopMovement(0f); // 0 = indefinitely
            if (action != null)
            {
                action(PlayerAction.MurdererAccused);
            }
        }
        else
        {
          //  GetComponent<Renderer>().material.color = Color.yellow;
            falseAccusationIndicator.SetActive(true);
            GetComponent<PlayerMovement>().StopMovement(10f);
            Invoke("ResetColor", 10f);
            if (action != null)
            {
                action(PlayerAction.PlayerAccused);
            }
        }
    }

    void ResetColor()
    {
       // GetComponent<Renderer>().material.color = IsDetective ? Color.blue : Color.white;
        falseAccusationIndicator.SetActive(false);
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
   //         GetComponent<Renderer>().material.color = Color.red;

            bunnyModel.SetActive(false);
            wolfModel.SetActive(false);

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
    void recieveShove(Vector3 otherPlayerPostion)
    {
        if (!photonView.isMine) return;

        inventory.recieveItem(ItemPickups.Nothing);
        gameObject.GetComponent<PlayerMovement>().recieveShove((gameObject.transform.position - otherPlayerPostion));
        
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