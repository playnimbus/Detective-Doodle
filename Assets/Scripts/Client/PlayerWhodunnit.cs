using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerWhodunnit : Player
{
    public GameObject falseAccusationIndicator;

    public bool IsMurderer { get; private set; }
    public bool IsDetective { get; private set; }

    private bool canMurder;

    PlayerWhodunnit otherPlayerWhodunnit;

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);

        MakeDetective();        //used for no detective game. Make everyone detective
                                //comment out to re-enable detectives
                                //also uncomment code from whoDunnitMasterSession
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

    public override void OnCollisionEnter(Collision collision)
    {
        
        base.OnCollisionEnter(collision);

        if (!photonView.isMine) return;
        otherPlayerWhodunnit = collision.gameObject.GetComponent<PlayerWhodunnit>();
        if (otherPlayerWhodunnit == null) return;

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
    }

    public override void OnCollisionExit(Collision collision)
    {
       
        if (!photonView.isMine) return;
        PlayerWhodunnit other = collision.gameObject.GetComponent<PlayerWhodunnit>();

        if (other != null)
        {
        }

        base.OnCollisionExit(collision);
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

        ui.ShowButton(0, "Shove", true, () =>
        {
            murderButton = 1;

            otherPlayerWhodunnit.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            //photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
        });

        // Now our murder business
        if (canMurder)
        {
            ui.ShowButton(1, "Murder", true, () =>
            {
                otherPlayerWhodunnit.photonView.RPC("Kill", PhotonTargets.All);
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
        int detectiveButton = 0;

        if (inventory.ItemInHand == ItemPickups.Evidence)
        {
            ui.ShowButton(0, "Accuse", true, () =>
            {
                detectiveButton = 1;
                otherPlayerWhodunnit.photonView.RPC("Accuse", PhotonTargets.All);
                inventory.removeItem();

                // If we're wrong we also are punished!
                if (!otherPlayerWhodunnit.IsMurderer) photonView.RPC("Accuse", PhotonTargets.All);

                Analytics.PlayerAccused(otherPlayerWhodunnit.IsMurderer);
            });

            ui.ShowButton(1, "Shove", true, () =>
            {
                otherPlayerWhodunnit.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });

            return true;
        }

        ui.ShowButton(0, "Shove", true, () =>
        {
            otherPlayerWhodunnit.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
        });



        return false;
    }

    bool BystanderInteraction()
    {
        if (otherPlayerWhodunnit.IsDetective && !otherPlayerWhodunnit.IsDead)
        {
            ui.ShowButton(0, "Give Evidence", true, () =>
            {
                otherPlayerWhodunnit.inventory.recieveItem(ItemPickups.Evidence);
                inventory.removeItem();
            });
            return true;
        }

        if (otherPlayerWhodunnit.IsDetective && otherPlayerWhodunnit.IsDead)
        {
            ui.ShowButton(0, "Become Detective", true, () =>
            {
                otherPlayerWhodunnit.photonView.RPC("RemoveDetectiveship", PhotonTargets.All);
                photonView.RPC("MakeDetective", PhotonTargets.All);
            });
            return true;
        }

        return false;
    }

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

    //should be moved into base Player script
    [RPC]
    void recieveShove(Vector3 otherPlayerPostion)
    {
        if (!photonView.isMine) return;

        inventory.recieveItem(ItemPickups.Nothing);
        gameObject.GetComponent<PlayerMovement>().recieveShove((gameObject.transform.position - otherPlayerPostion));

    }

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