using UnityEngine;
using System.Collections;

public class MaxSpeed : Powerup {

    public float duration;
    public float speed;

    public override bool Apply(Player player){

        StartCoroutine(StartPowerup(player));
        return true;
    }

    IEnumerator StartPowerup(Player player)
    {
        float originalSpeed = player.GetComponent<PlayerMovement>().speed;
        player.GetComponent<PlayerMovement>().speed = speed;
        
        yield return new WaitForSeconds(duration);

        player.GetComponent<PlayerMovement>().speed = originalSpeed;

        Destroy(this);
    }
}
