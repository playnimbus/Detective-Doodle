using UnityEngine;
using System.Collections;

public class PlayerMovement : Photon.MonoBehaviour
{
    public float speed;
    public float virtualScreenPadRadius;

    private Coroutine moveCoroutine;
	public PlayerCamera playerCamera;

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

        while (Input.GetMouseButton(0))
        {
            Vector3 current = Input.mousePosition;
            Vector3 delta = current - startPosition;
            delta.z = delta.y;
            delta.y = 0;
            float length = Mathf.Min(delta.magnitude, virtualScreenPadRadius);

            delta.Normalize();
            float scale = CubicInOut(length, 0, 1, virtualScreenPadRadius);
            velocity = delta * scale;

			playerCamera.playerDirection = delta;

            // Move using rigidbody to get collision benefits
            GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime * speed);

            yield return fixedUpdate;
        }

        // Slow to a stop
        while (true)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.15f);

            // Move using rigidbody to get collision benefits
            GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime * speed);

            yield return fixedUpdate;
        }
    }

    float CubicInOut(float t, float b, float c, float d)
    {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t * t + b;
        t -= 2;
        return c / 2 * (t * t * t + 2) + b;
    }
}