using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{
    public float speed;
    public float virtualScreenPadRadius;
        
    private new PlayerCamera camera;
    public PlayerCamera Camera { get { return camera; } }

    private Coroutine stashCorouitine;

    // Acts as a Start() for network instantiated objects
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        InitCamera();
    }
    
    void InitCamera()
    {
        camera = GetComponentInChildren<PlayerCamera>();
        if (camera == null) Debug.LogError("[Player] Couldn't find PlayerCamera", this);

        if (photonView.isMine)
        {
            //camera.transform.parent = null;
            transform.DetachChildren();
            camera.Init(this.transform);
        }
        else Destroy(camera.gameObject);
    }

    public void AppraochedStash(EvidenceStash stash)
    {
        stashCorouitine = StartCoroutine(NearbyStashCorouitine(stash));
    }

    public void LeftStash(EvidenceStash stash)
    {
        StopCoroutine(stashCorouitine);
        stashCorouitine = null;
    }

    IEnumerator NearbyStashCorouitine(EvidenceStash stash)
    {
        while(true)
        {
            // Double click detection
            if(Input.GetMouseButtonDown(0))
            {
                float endTime = Time.time + 0.5f;
                while(Time.time < endTime)
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        // Loot the stash
                        Debug.Log("[Player] Player has looted the stash!");
                        yield break;
                    }
                    yield return null;
                }
            }
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && photonView.isMine)
        {
            StartCoroutine(MoveCoroutine());
        }
    }

    IEnumerator MoveCoroutine()
    {
        Vector3 startPosition = Input.mousePosition;

        while(Input.GetMouseButton(0))
        {
            Vector3 current = Input.mousePosition;
            Vector3 delta = current - startPosition;
            delta.z = delta.y;
            delta.y = 0;
            float length = Mathf.Min(delta.magnitude, virtualScreenPadRadius);
            
            delta.Normalize();
            float scale = CubicInOut(length, 0, 1, virtualScreenPadRadius);
            
            // Move using rigidbody to get collision benefits
            rigidbody.MovePosition(transform.position + delta * scale * Time.deltaTime * speed);

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
}