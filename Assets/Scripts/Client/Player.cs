using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{
    public float speed;

    void Update()
    {
        if (photonView.isMine)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        direction.Normalize();
        transform.Translate(direction * Time.deltaTime * speed, Space.Self);
    }
}