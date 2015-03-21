using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Photon.MonoBehaviour
{        
    private new PlayerCamera camera;
    private int evidencePiecesGathered;
    private PlayerUI ui;
    private bool isMurderer;

    // Acts as a Start() for network instantiated objects
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        evidencePiecesGathered = 0;
        InitUI();
        InitCamera();

        // HACK ... Kinda. Assumes we only one text field on the player
        GetComponentInChildren<Text>().text = photonView.owner.name;
    }

    void InitUI()
    {
        if (!photonView.isMine) return;

        GameObject menuGO = Instantiate(Resources.Load<GameObject>("ClientMenu")) as GameObject;
        ui = menuGO.GetComponent<PlayerUI>();
        ui.SetEvidenceText("Evidence gathered: " + evidencePiecesGathered);
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

    public void ApproachedStash(EvidenceStash stash)
    {
        if (!photonView.isMine) return;

        if (evidencePiecesGathered == 3)
        {
            ui.ShowButton("Already collected max evidence.", null);
        }
        else
        {
            ui.ShowButton("Search", () =>
                {
                    evidencePiecesGathered++;
                    ui.SetEvidenceText("Evidence gathered: " + evidencePiecesGathered);
                });
        }
    }

    public void LeftStash(EvidenceStash stash)
    {
        if (!photonView.isMine) return;

        ui.HideButton();
    }
    
    void EnteredRoom(LevelRoom room)
    {
        if (!photonView.isMine) return;

        camera.MoveToVantage(room.overheadCameraPosition);
        room.Reveal();
    }

    void ExitedRoom(LevelRoom room)
    {
        if (!photonView.isMine) return;

        camera.ResumeFollow();
        room.Conceal();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.isMine) return;
        Player other = collision.gameObject.GetComponent<Player>();
        if(other != null)
        {
            if (isMurderer)
            {
                ui.ShowButton("Poison", () =>
                    {
                        other.photonView.RPC("Kill", PhotonTargets.All);
                    });
            }
            else
            {
                if (evidencePiecesGathered == 3)
                {
                    ui.ShowButton("Accuse", () =>
                        {
                            other.photonView.RPC("Accuse", other.photonView.owner);
                            evidencePiecesGathered = 0;
                            ui.SetEvidenceText("Evidence gathered: " + evidencePiecesGathered);
                        });
                }
                else if(evidencePiecesGathered > 0)
                {
                    ui.ShowButton("Share Evidence", () =>
                        {
                            other.photonView.RPC("ShareEvidence", other.photonView.owner);
                        });
                }
                else
                {
                    ui.ShowButton("No Evidence To Share", null);
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!photonView.isMine) return;
        Player other = collision.gameObject.GetComponent<Player>();
        if(other != null)
        {
            ui.HideButton();
        }
    }

    [RPC]
    void Accuse()
    {
        // The murderer is vanquished!
        if (isMurderer)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
    
    [RPC]
    void Kill()
    {
        Destroy(gameObject, 20f);
        // Also share evidence so player isn't aware of poison...
        if (photonView.isMine) ShareEvidence();
    }

    [RPC]
    void ShareEvidence()
    {
        // Someone gave us some evidence
        if (evidencePiecesGathered < 3)
        {
            evidencePiecesGathered++;
            ui.SetEvidenceText("Evidence gathered: " + evidencePiecesGathered);
        }
    }

    public void MakeMurderer()
    {
        isMurderer = true;
        ui.MarkAsMurderer();        
    }
}