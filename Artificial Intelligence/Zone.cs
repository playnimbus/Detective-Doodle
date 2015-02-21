using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * stores information for the zones. connections, nodes contained in the zone, status, as well as pathfinding info.
 */

public class Zone : MonoBehaviour {

    public enum Status { open = 0, check = 1, closed = 2 };

    public GameObject[] Connections;
    public List<Node> NodesInZone;

    //used for pathfinding
    public Status status = Status.open;
    public float costFromStart = 0;
    public float costToDestination = 0;
    public float totalCost = 0;
    public GameObject parent = null;
    //-------------------------------------

    /*
     * builds the connections between the nodes in the zone that are not along the edge of the zone.
     * it builds the connectins based off of their static positions in the generated list.
     */
    public List<Node> buildNodeConnections()
    {
        Vector3 bounds = GetComponent<BoxCollider>().bounds.size;
        List<Node> edgeNodes = new List<Node>();

        int convert = 0;
        convert = (int)bounds.x;
        bounds.x = convert;
        convert = (int)bounds.y;
        bounds.y = convert; 
        convert = (int)bounds.z;
        bounds.z = convert;

        int rowCounter = 0;

        for (int i = 0; i < NodesInZone.Count; i++)
        {
            if (i < bounds.z)   //marking edge
            {
                NodesInZone[i].gizmoColor = Color.blue;
                edgeNodes.Add(NodesInZone[i]);
            }
            else if (i >= NodesInZone.Count - bounds.z)  //marking edge
            {
                NodesInZone[i].gizmoColor = Color.blue;
                edgeNodes.Add(NodesInZone[i]);
            }
            else if (i % (bounds.z + 1) == 0)   //marking edge
            {
                NodesInZone[i].gizmoColor = Color.green;
                edgeNodes.Add(NodesInZone[i]);
            }

            else if ((i - rowCounter) % (bounds.z) == 0)    //marking edge
            {
                NodesInZone[i].gizmoColor = Color.magenta;
                rowCounter++;
                edgeNodes.Add(NodesInZone[i]);
            }

            else     //not an edge, so connect it to neighboring nodes
            {
                NodesInZone[i].gizmoColor = Color.grey;
                NodesInZone[i].Connections = new Node[8];
                NodesInZone[i].Connections[0] = NodesInZone[i + 1];
                NodesInZone[i].Connections[1] = NodesInZone[i + -1];
                NodesInZone[i].Connections[2] = NodesInZone[i + (int)bounds.z];
                NodesInZone[i].Connections[3] = NodesInZone[i - (int)bounds.z];
                NodesInZone[i].Connections[4] = NodesInZone[i - ((int)bounds.z + 2)]; 
                NodesInZone[i].Connections[5] = NodesInZone[i - ((int)bounds.z + 1)];
                NodesInZone[i].Connections[6] = NodesInZone[i + ((int)bounds.z + 1)];
                NodesInZone[i].Connections[7] = NodesInZone[i + ((int)bounds.z + 2)];
                NodesInZone[i].numConnections = 8;
            }

        }
        return edgeNodes;
    }
    //resets the zone after pathfinding
    public void resetZone()
    {
        status = Status.open;
        costFromStart = 0;
        costToDestination = 0;
        totalCost = 0;
        parent = null;
    }

    public void setStatus(Status _status) { status = _status; }
    public Status getStatus() { return status; }

    public void setCostFromStart(float _cost) { costFromStart = _cost; }
    public void addCostFromStart(float _cost) { costFromStart += _cost; }
    public float getCostFromStart() { return costFromStart; }

    public void setCostToDestination(float _cost) { costToDestination = _cost; }
    public void addCostToDestination(float _cost) { costToDestination += _cost; }
    public float getCostToDestination() { return costToDestination; }

    public void setTotalCost(float _cost) { totalCost = _cost; }
    public float getTotalCost() { return totalCost; }

    public void setParent(GameObject _parent) { parent = _parent; }
    public GameObject getParent() { return parent; }

}
