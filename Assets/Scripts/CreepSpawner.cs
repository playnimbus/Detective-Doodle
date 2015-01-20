using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * spawns entities at the location of the spawner when the 'm' key is pressed
 */

public class CreepSpawner : MonoBehaviour {

    public float spawnRate;
    float time;

    public GameObject closestZone;
    public Node creepStartingNode;

    int maxCreep = 1;
    int numCreep = 0;
	// Use this for initialization

    //finds the closest node to the spawner to properly set the creeps starting node
	void Start () {
        float currentClosestValue = 9999;
        for (int i = 0; i < closestZone.GetComponent<Zone>().NodesInZone.Count; i++)
        {
            if( Vector3.Distance(closestZone.GetComponent<Zone>().NodesInZone[i].position, gameObject.transform.position) < currentClosestValue)
            {
                currentClosestValue = Vector3.Distance(closestZone.GetComponent<Zone>().NodesInZone[i].position, gameObject.transform.position);
                creepStartingNode = closestZone.GetComponent<Zone>().NodesInZone[i];
            }
        }

        spawnCreep();
	}

    // Update is called once per frame
    void Update()
    {
//uncomment section for auto spawning entities

  //      time += Time.deltaTime;
   /*   
        if (time >= spawnRate && numCreep < maxCreep)
        {
            GameObject creep = (GameObject)Instantiate(Resources.Load("skeleton"));
            creep.transform.position = gameObject.transform.position;
            time = 0;
            numCreep++;
        }
*/
        if (Input.GetKeyDown("m"))
        {
            spawnCreep();
        }
	}

    public void minusCreep()
    {
        numCreep--;
    }
    public void spawnCreep()
    {
        GameObject creep = (GameObject)Instantiate(Resources.Load("entity"));
        creep.transform.position = gameObject.transform.position;
        creep.GetComponent<CreepController>().setCurrentNode(creepStartingNode);
        numCreep++;
    }
}
