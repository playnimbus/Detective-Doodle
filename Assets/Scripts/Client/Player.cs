using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{
    public float speed;
    public float virtualScreenPadRadius;

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!photonView.isMine)
            Destroy(GetComponentInChildren<Camera>().gameObject);
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

            transform.Translate(delta * scale * Time.deltaTime * speed, Space.World);

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