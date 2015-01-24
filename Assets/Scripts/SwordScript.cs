using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            Debug.Log("playerAttacked");
            col.collider.SendMessage("Attacked");
        }
    }
}
