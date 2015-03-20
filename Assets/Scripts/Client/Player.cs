using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : Photon.MonoBehaviour
{
    public float speed;
    public float virtualScreenPadRadius;
        
    private new PlayerCamera camera;
    public PlayerCamera Camera { get { return camera; } }

    private int evidencePiecesGathered;
    private PlayerUI ui;
    private Coroutine moveCoroutine;

    public bool isMurderer;

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
        if (photonView.isMine)
        {
            GameObject menuGO = Instantiate(Resources.Load<GameObject>("ClientMenu")) as GameObject;
            ui = menuGO.GetComponent<PlayerUI>();
            ui.SetEvidenceText("Evidence gathered: " + evidencePiecesGathered);
        }
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
        //else Destroy(camera.gameObject);
        else camera.camera.enabled = false;
    }

    public void ApproachedStash(EvidenceStash stash)
    {
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
        ui.HideButton();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && photonView.isMine)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveCoroutine());
        }
    }

    IEnumerator MoveCoroutine()
    {
        Vector3 startPosition = Input.mousePosition;
        Vector3 velocity = Vector3.zero;

        // Use fixed update to play nicely with physics
        WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

        while(Input.GetMouseButton(0))
        {
            Vector3 current = Input.mousePosition;
            Vector3 delta = current - startPosition;
            delta.z = delta.y;
            delta.y = 0;
            float length = Mathf.Min(delta.magnitude, virtualScreenPadRadius);
            
            delta.Normalize();
            float scale = CubicInOut(length, 0, 1, virtualScreenPadRadius);
            velocity = delta * scale;
            
            // Move using rigidbody to get collision benefits
            rigidbody.MovePosition(transform.position + velocity * Time.deltaTime * speed);

            yield return fixedUpdate;
        }

        // Slow to a stop
        while(true)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.15f);

            // Move using rigidbody to get collision benefits
            rigidbody.MovePosition(transform.position + velocity * Time.deltaTime * speed);

            yield return fixedUpdate;
        }
    }

    float CubicInOut(float t, float b, float c, float d)
    {
	    t /= d/2;
	    if (t < 1) return c/2*t*t*t + b;
	    t -= 2;
	    return c/2*(t*t*t + 2) + b;
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