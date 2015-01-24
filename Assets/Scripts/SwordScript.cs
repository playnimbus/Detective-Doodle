using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

    public string playerName;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            if (col.collider.name != playerName)
            {
                Debug.Log("playerAttacked");
                col.collider.SendMessage("Attacked");
            }
        }
    }
}
