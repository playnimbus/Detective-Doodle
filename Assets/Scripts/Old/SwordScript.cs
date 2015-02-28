using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

    public string playerName;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.name != playerName)
            {
                Debug.Log("playerAttacked " + playerName);
             //   col.gameObject.SendMessage("Attacked");
                col.gameObject.GetComponent<playerController>().Attacked();
            }
        }
    }
}
