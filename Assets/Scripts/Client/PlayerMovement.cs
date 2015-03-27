using UnityEngine;
using System.Collections;

public class PlayerMovement : Photon.MonoBehaviour
{
    public float speed;
    public float virtualScreenPadRadius;

    private Coroutine moveCoroutine;
    private bool canMove = true;
	public PlayerCamera playerCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && photonView.isMine && canMove)
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

        // Wait till input is out of dead zone :]
        while (Input.GetMouseButton(0) && Vector3.Distance(startPosition, Input.mousePosition) < 25)
            yield return fixedUpdate;

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

            playerCamera.playerDirection = velocity;

            if (canMove)
            {
                // Move using rigidbody to get collision benefits
                GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime * speed);
            }

            yield return fixedUpdate;
        }
        
        // Slow to a stop
        while (true)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, 0.15f);

            playerCamera.playerDirection = velocity;

            if (canMove)
            {
                // Move using rigidbody to get collision benefits
                GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime * speed);
            }

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

    public void StopMovement(float seconds)
    {
        canMove = false;

        // Stop any pending movement changes
        CancelInvoke("ResumeMovement");

        if (seconds != 0f)
            Invoke("ResumeMovement", seconds);
            
    }

    void ResumeMovement()
    {
        canMove = true;
    }
}