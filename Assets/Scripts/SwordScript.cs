using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

    void OnTriggerEnter(Collider trg)
    {
        if (trg.collider.tag == "Player")
        {
            trg.collider.SendMessage("Attacked");
        }
    }
}
