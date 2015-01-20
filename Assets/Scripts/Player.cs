using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Player Script handles setting the players health, and initializing the players starting node while also keeping track of his current node throughout the game
 */

public class Player : MonoBehaviour {

    public int health = 20;
    public GameObject healthGuiTxt;

    public Node currentNode;

	// Use this for initialization
	void Start () {

        healthGuiTxt.guiText.text = health.ToString();
        findStartingNode();
	}
	
	// Update is called once per frame
	void Update () {

        checkCurrentNode();
	}

    //called when enemy lands an attack
    public void applyDamage(int value)
    {
        health -= value;
        healthGuiTxt.guiText.text = health.ToString();
    }

    //checks your current nodes connections to see if you are closer to a connection.
    //as you move around the level this method keeps track of what node you are on.
    void checkCurrentNode()
    {
        Node[] currentConnections = currentNode.Connections;

        Node closest = currentNode;
        float closestDistance = Vector3.Distance(gameObject.transform.position, currentNode.position);

        for (int i = 0; i < currentNode.numConnections; i++)
        {
            float currentDistance = Vector3.Distance(gameObject.transform.position, currentConnections[i].position);
            if (closestDistance > currentDistance && currentConnections[i].status != Node.Status.closed)
            {
                closestDistance = currentDistance;
                closest = currentConnections[i];
            }
        }
        setCurrentNode(closest);        
    }

    //finds the closest node to you when you start the level
    void findStartingNode()
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag("Zone");

        GameObject closest = zones[0];
        float closestDistance = Vector3.Distance(gameObject.transform.position, zones[0].transform.position);

        //find zone
        for (int i = 1; i < zones.Length; i++)
        {
            float currentDistance = Vector3.Distance(gameObject.transform.position, zones[i].transform.position);
            if (closestDistance > currentDistance)
            {
                closestDistance = currentDistance;
                closest = zones[i];
            }
        }

        List<Node> nodes = closest.GetComponent<Zone>().NodesInZone;
        Node closestNode = nodes[0];

        closestDistance = Vector3.Distance(gameObject.transform.position, nodes[0].position);

        //find node in zone
        for (int i = 1; i < nodes.Count; i++)
        {
            float currentDistance = Vector3.Distance(gameObject.transform.position, nodes[i].position);
            if (closestDistance > currentDistance)
            {
                closestDistance = currentDistance;
                closestNode = nodes[i];
            }
        }
        setCurrentNode(closestNode);
    }

    public void setCurrentNode(Node _node)
    {
        currentNode = _node;
    }


    /*
void OnTriggerEnter(Collider other)
{
    if (other.transform.tag == "Node")
    {
        if (other.GetComponent<Node>().getStatus() != Node.Status.closed)
        {
            currentNode = other.gameObject;
        }
    }
}
 * */

}
