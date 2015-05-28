using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerMonster : Player
{
    PlayerMonster otherPlayerMonster;

    public bool IsMonster { get; private set; }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);

     //   if (!photonView.isMine) return;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (!photonView.isMine) return;
        otherPlayerMonster = collision.gameObject.GetComponent<PlayerMonster>();
        if (otherPlayerMonster == null) return;

        // return when you do an action!
        if (IsMonster)
        {
            if (MonsterInteractions()) return;
        }
        else // Is bystander
        {
            if (BystanderInteraction()) return;
        }
    }

    public override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);

        if (!photonView.isMine) return;
        PlayerMonster other = collision.gameObject.GetComponent<PlayerMonster>();

        if (other != null)
        {
        }
    }

    bool BystanderInteraction()
    {
     //   if (inventory.ItemInHand == ItemPickups.Evidence)
     //   {
            ui.ShowButton(0, "Accuse", true, () =>
            {
                otherPlayerMonster.photonView.RPC("Accuse", PhotonTargets.All);
                inventory.removeItem();

                // If we're wrong we also are punished!
                if (!otherPlayerMonster.IsMonster) photonView.RPC("Accuse", PhotonTargets.All);

                Analytics.PlayerAccused(otherPlayerMonster.IsMonster);
            });

            ui.ShowButton(1, "Shove", true, () =>
            {
                otherPlayerMonster.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            });

            return true;
    //    }

        ui.ShowButton(0, "Shove", true, () =>
        {
            otherPlayerMonster.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
        });



        return false;
    }

    bool MonsterInteractions()
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

            otherPlayerMonster.photonView.RPC("recieveShove", PhotonTargets.All, gameObject.transform.position);
            //photonView.RPC("SetHaveEvidence", PhotonTargets.All, false);
        });

        return interacted;
    }

    [RPC]
    public void MakeMonster()
    {
        IsMonster = true;

        if (photonView.isMine)
        {
            ui.MarkAsMurderer();
        }
    }

    //should be moved into base Player script
    [RPC]
    void recieveShove(Vector3 otherPlayerPostion)
    {
        if (!photonView.isMine) return;

        inventory.recieveItem((GameObject)null);
        gameObject.GetComponent<PlayerMovement>().recieveShove((gameObject.transform.position - otherPlayerPostion));

    }

}