using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

    public GameObject playerCursor;
    public float playerSpeed;

    public GameObject meleeGO;
    float meleeTimer = 0;
    float meleeUpTime = 0.2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        MovementAnalog();

        if (meleeGO.active == true)
        {
            meleeTimer += Time.deltaTime;
            if (meleeTimer >= meleeUpTime)
            {
                meleeGO.SetActive(false);
                meleeTimer = 0;
            }
        }
	}

    public void SwingSword()
    {
        meleeGO.SetActive(true);
    }

    public void MovementAnalog()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "AnalogStick")
                {
                    Vector3 normalizedCastPosition = hit.point - hit.transform.position;
                    Vector3 forceToAdd = new Vector3(((hit.point.x - hit.transform.position.x) * playerSpeed), 0, ((hit.point.z - hit.transform.position.z) * playerSpeed));
                    gameObject.rigidbody.AddForce(forceToAdd);
                    transform.LookAt(gameObject.transform.position + forceToAdd);
                }
            }
        }
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
            rigidbody.velocity = transform.forward * playerSpeed;
        }
        //upright
        else if(direction == 2)
        {
            rigidbody.velocity = (transform.forward + transform.right) * playerSpeed;
        }
        //right
        else if (direction == 3)
        {
            rigidbody.velocity = transform.right * playerSpeed;
        }
        //downright
        else if (direction == 4)
        {
            rigidbody.velocity = (-transform.forward + transform.right) * playerSpeed;
        }
        //down
        else if (direction == 5)
        {
            rigidbody.velocity = -transform.forward * playerSpeed;
        }
        //downleft
        else if (direction == 6)
        {
            rigidbody.velocity = (-transform.forward + -transform.right) * playerSpeed;
        }
        //left
        else if (direction == 7)
        {
            rigidbody.velocity = -transform.right * playerSpeed;
        }
        //upleft
        else if (direction == 8)
        {
            rigidbody.velocity = (transform.forward + -transform.right) * playerSpeed;
        }
        //stop
        else if (direction == 0)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}