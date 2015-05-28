using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Player : Photon.MonoBehaviour
{
    public GameObject bunnyModel;
    public GameObject wolfModel;
    public Text name;

    LootMicroGame LootDrawer;
    LockedDoorUI lockedDoorUI;

    public bool IsDead { get; set; }

    public static class PlayerAction
    { 
        public const byte MurdererAccused = 0;
        public const byte PlayerKilled = 1;
        public const byte PlayerAccused = 2;
    }
    public Action<byte> action;

    private new PlayerCamera camera;
    public PlayerUI ui;
    public PlayerInventory inventory;
    private Coroutine stashSearch;
    private AudioBank audio;

    public AudioBank Audio { set { audio = value; } }
    Player otherPlayer;

    #region Initialization

    // Acts as a Start() for network instantiated objects
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        InitUI();
        InitCamera();

        inventory = gameObject.GetComponent<PlayerInventory>();

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

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (!photonView.isMine) return;
        otherPlayer = collision.gameObject.GetComponent<Player>();
        if (otherPlayer == null) return;

        // All players can loot dead 
        if (LootingInteraction()) return;
    }

    public virtual void OnCollisionExit(Collision collision)
    {
        if (!photonView.isMine) return;
        Player other = collision.gameObject.GetComponent<Player>();
        if (other != null)
        {
            ui.HideAllButtons();
        }
    }

    bool LootingInteraction()
    {
        /*
        if (otherPlayer.IsDead && otherPlayer.inventory.ItemInHand != ItemPickups.Nothing)
        {
            ui.ShowButton(0, "Take Item", true, () =>
            {
                inventory.recieveItem(otherPlayer.inventory.ItemInHand);
                otherPlayer.inventory.removeItem();
            });
            return true;
        } */
        return false;
        
    }

    #endregion

}