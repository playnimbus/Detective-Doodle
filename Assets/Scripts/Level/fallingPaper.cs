using UnityEngine;
using System.Collections;

public class fallingPaper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(0, 2), Random.Range(5, 8), Random.Range(-1, -3));
        gameObject.GetComponent<Rigidbody>().AddRelativeTorque(0, Random.Range(50, 100), 0);

        Destroy(gameObject, 5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
