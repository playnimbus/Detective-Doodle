using UnityEngine;
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
        transform.DetachChildren();
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
        }
        else Destroy(camera.gameObject);
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

            yield return null;
        }

        // Slow to a stop
        while(true)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.15f);

            // Move using rigidbody to get collision benefits
            rigidbody.MovePosition(transform.position + velocity * Time.deltaTime * speed);

            yield return null;
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
                        });
                }
                else
                {
                    ui.ShowButton("Share Evidence", () =>
                        {
                            other.photonView.RPC("ShareEvidence", other.photonView.owner);
                        });
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
        if (isMurderer) Destroy(gameObject, 2f);
    }
    
    [RPC]
    void Kill()
    {
        Destroy(gameObject, 20f);
        // Also share evidence so player isn't aware of poison...
        ShareEvidence();
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