using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * No longer beings used
 * 
 * ConnectionCaster is a method for building the connections between nodes useing raycasting. this method requires the nodes to
 * have box colliders which is not ideal and therefor is not being used with the scriptable object nodes.
 */

public class ConnectionCaster : MonoBehaviour {

    Vector3 direction1 = Vector3.forward;
    Vector3 direction2 = Vector3.right;
    Vector3 direction3 =-Vector3.forward;
    Vector3 direction4 =-Vector3.right;
    Vector3 direction5 = Vector3.forward + Vector3.right;
    Vector3 direction6 = Vector3.forward - Vector3.right;
    Vector3 direction7 = -Vector3.forward + Vector3.right;
    Vector3 direction8 = -Vector3.forward - Vector3.right;

	// Update is called once per frame
	void Update () {
 /*
       Debug.DrawRay(transform.position, direction1, Color.green);
       Debug.DrawRay(transform.position, direction2 , Color.green);
       Debug.DrawRay(transform.position, direction3, Color.green);
       Debug.DrawRay(transform.position, direction4, Color.green);
       Debug.DrawRay(transform.position, direction5, Color.green);
       Debug.DrawRay(transform.position, direction6, Color.green);
       Debug.DrawRay(transform.position, direction7, Color.green);
       Debug.DrawRay(transform.position, direction8, Color.green);
        * */

	}
    /*
    public Node[] getConnectionsByDistance(GameObject _zone, string _name)
    {
        Node[] Connections;
        Connections = new Node[8];
        int connectionIndex = 0;

        //check for nodes within zone
        for (int i = 0; i < _zone.GetComponent<Zone>().NodesInZone.Count; i++)
        {
            if (Mathf.Abs(Vector3.Distance(_zone.GetComponent<Zone>().NodesInZone[i].position, gameObject.transform.position)) <= 1.75f
                && _zone.GetComponent<Zone>().NodesInZone[i].name != _name)
            {
                if (connectionIndex < 8)
                {
                    Connections[connectionIndex] = _zone.GetComponent<Zone>().NodesInZone[i];
                    connectionIndex++;
                }
                else
                {
                    int indexOfFurthest = 0;
                    float furthestDistance = Mathf.Abs(Vector3.Distance(Connections[indexOfFurthest].position, gameObject.transform.position));

                    for (int j = 0; j < Connections.Length; j++)
                    {
                        float distance = Mathf.Abs(Vector3.Distance(Connections[j].transform.position, gameObject.transform.position));

                        if (distance > furthestDistance)
                        {
                            indexOfFurthest = j;
                            furthestDistance = distance;
                        }
                    }
                    Connections[indexOfFurthest] = _zone.GetComponent<Zone>().NodesInZone[i];
                }
            }
        }

        //check for nodes in neighbor zones
        for (int i = 0; i < _zone.GetComponent<Zone>().numConnections; i++)
        {
            for (int x = 0; x < _zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone.Count; x++)
            {
                if (Mathf.Abs(Vector3.Distance(_zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x].transform.position, gameObject.transform.position)) <= 1.75f
                        && _zone.GetComponent<Zone>().NodesInZone[i].name != _name)
                {
                    if (connectionIndex < 8)
                    {
                        Connections[connectionIndex] = _zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x];
                        connectionIndex++;
                    }
                    else
                    {
                        int indexOfFurthest = 0;
                        float furthestDistance = Mathf.Abs(Vector3.Distance(Connections[indexOfFurthest].transform.position, gameObject.transform.position));

                        for (int j = 0; j < Connections.Length; j++)
                        {
                            float distance = Mathf.Abs(Vector3.Distance(Connections[j].transform.position, gameObject.transform.position));

                            if (distance > furthestDistance)
                            {
                                indexOfFurthest = j;
                                furthestDistance = distance;
                            }
                        }
                        Connections[indexOfFurthest] = _zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x];
                    }
                }
            }
        }

        return Connections;
    }
    */
    /*
    public Node[] getConnections()
    {
        Node[] Connections;
        Connections = new Node[8];
        int connectionIndex = 0;

        int layerMask = 1 << 8;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction1, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction2, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction3, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction4, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction5, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction6, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction7, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }

        if (Physics.Raycast(transform.position, direction8, out hit, 5, layerMask))
        {
            if (hit.transform.tag == "Node")
            {
                Connections[connectionIndex] = hit.transform.gameObject.GetComponent<Node>();
                connectionIndex++;
            }
        }


        return Connections;
    }
     * */
}
