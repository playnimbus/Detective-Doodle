using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

    public GameObject playerCursor;
    public float playerSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Movement(int direction)
    {
        //Touch and move in anydirection
        //Vector3 direction = playerCursor.transform.position - gameObject.transform.position;
        //playerCursor.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        //rigidbody2D.velocity = direction * speed;

        //Movement with GUI_D-Pad
        //up
        if (direction == 1)
        {
            rigidbody2D.velocity = transform.up * playerSpeed;
        }
        //upright
        else if(direction == 2)
        {
            rigidbody2D.velocity = (transform.up + transform.right) * playerSpeed;
        }
        //right
        else if (direction == 3)
        {
            rigidbody2D.velocity = transform.right * playerSpeed;
        }
        //downright
        else if (direction == 4)
        {
            rigidbody2D.velocity = (-transform.up + transform.right) * playerSpeed;
        }
        //down
        else if (direction == 5)
        {
            rigidbody2D.velocity = -transform.up * playerSpeed;
        }
        //downleft
        else if (direction == 6)
        {
            rigidbody2D.velocity = (-transform.up + -transform.right) * playerSpeed;
        }
        //left
        else if (direction == 7)
        {
            rigidbody2D.velocity = -transform.right * playerSpeed;
        }
        //upleft
        else if (direction == 8)
        {
            rigidbody2D.velocity = (transform.up + -transform.right) * playerSpeed;
        }
        //stop
        else if (direction == 0)
        {
            rigidbody2D.velocity = Vector3.zero;
        }
    }
}