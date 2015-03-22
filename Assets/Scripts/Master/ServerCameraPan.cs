using UnityEngine;
using System.Collections;

public class ServerCameraPan : MonoBehaviour {

    float timer = 0;
    float rotationAmount = 0.05f;
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if (timer >= 13)
        {
            rotationAmount *= -1;
            timer = 0;
        }

        this.transform.localEulerAngles += new Vector3(0, rotationAmount, 0);
	}
}
