using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Spinner : MonoBehaviour 
{
    public float spinRate;

    private float startTime;

    void Start()
    {
        startTime = Time.time;    
    }

    void Update()
    {
        //this.transform.localRotation = Quaternion.EulerAngles(0, 0, Mathf.Sin(Time.time
        this.transform.Rotate(0, 0, spinRate * Mathf.Sin((Time.time - startTime) * 0.5f + Mathf.PI / 2f), Space.Self);
    }
}
