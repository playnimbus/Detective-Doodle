using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Node class stores all the information required for the nodes in the pathfinding grid.
 */

public class Node : ScriptableObject {

    public enum Status { open = 0, check = 1, closed = 2 };

    public Vector3 position;
    public string name;

    public Node[] Connections;
    public int numConnections;
    public GameObject zone;

    //used for A* pathfinding
    public Status status = Status.open;
    public float costFromStart = 0;
    public float costToDestination = 0;
    public float totalCost = 0;
    public Node parent = null;
    public bool occupied = false;   //used for to tag the node if open or not from level geometry
    //-------------------------------------

    public Color gizmoColor = Color.yellow;

    //calls the getConnectionsByDistance method and sets the number of connections this node has.
    public void getNodesConnections()
    {
        Connections = getConnectionsByDistance();
        numConnections = 0;

        for (int i = 0; i < Connections.Length; i++)
        {
            if (Connections[i] != null)
            {
                numConnections++;
            }
        }
    }
    //resets pathfinding info after path has been found
    public void resetNode()
    {
        status = Status.open;
        costFromStart = 0;
        costToDestination = 0;
        totalCost = 0;
        parent = null;
        
        if (occupied == true)
        {
            status = Status.closed;
        }
    }

    //sphere casts up from the node to see if the level would block access to this node. then sets its occupied status.
    public void checkForLevelGeometry()
    {
        occupied = false;
        status = Status.open;
        
        RaycastHit hit;
        if (Physics.SphereCast(position,0.5f, Vector3.up, out hit,4.5f))
        {
            if (hit.transform.tag == "LevelGeometry")
            {
                occupied = true;
                status = Status.closed;
                gizmoColor = Color.red;
            }
        }
    }


    /*
     * used for the nodes on the edge of the zone grids to find the connections between zones.
     * it handles this by distance checking and finding the 7 closest nodes within the current zone and neighboring zone.
     * */
    public Node[] getConnectionsByDistance()
    {
        Node[] Connections;
        Connections = new Node[8];
        int connectionIndex = 0;

        //check for nodes within zone
        for (int i = 0; i < zone.GetComponent<Zone>().NodesInZone.Count; i++)
        {
            if (Mathf.Abs(Vector3.Distance(zone.GetComponent<Zone>().NodesInZone[i].position, position)) <= 1.75f
                && zone.GetComponent<Zone>().NodesInZone[i].name != name)
            {
                if (connectionIndex < 8)
                {
                    Connections[connectionIndex] = zone.GetComponent<Zone>().NodesInZone[i];
                    connectionIndex++;
                }
                else
                {
                    int indexOfFurthest = 0;
                    float furthestDistance = Mathf.Abs(Vector3.Distance(Connections[indexOfFurthest].position, position));

                    for (int j = 0; j < Connections.Length; j++)
                    {
                        float distance = Mathf.Abs(Vector3.Distance(Connections[j].position, position));

                        if (distance > furthestDistance)
                        {
                            indexOfFurthest = j;
                            furthestDistance = distance;
                        }
                    }
                    Connections[indexOfFurthest] = zone.GetComponent<Zone>().NodesInZone[i];
                }
            }
        }

        //check for nodes in neighbor zones
        for (int i = 0; i < zone.GetComponent<Zone>().Connections.Length; i++)
        {
            for (int x = 0; x < zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone.Count; x++)
            {
                
                if (Mathf.Abs(Vector3.Distance(zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x].position, position)) <= 1.75f
                        && zone.GetComponent<Zone>().NodesInZone[i].name != name)
                {
                    if (connectionIndex < 8)
                    {
                        Connections[connectionIndex] = zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x];
                        connectionIndex++;
                    }
                    else
                    {
                        int indexOfFurthest = 0;
                        float furthestDistance = Mathf.Abs(Vector3.Distance(Connections[indexOfFurthest].position, position));

                        for (int j = 0; j < Connections.Length; j++)
                        {
                            float distance = Mathf.Abs(Vector3.Distance(Connections[j].position, position));

                            if (distance > furthestDistance)
                            {
                                indexOfFurthest = j;
                                furthestDistance = distance;
                            }
                        }
                        Connections[indexOfFurthest] = zone.GetComponent<Zone>().Connections[i].GetComponent<Zone>().NodesInZone[x];
                    }
                }
            }
        }


        return Connections;
    }

    public Node[] getConnections() { return Connections; }

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

    public void setParent(Node _parent) { parent = _parent; }
    public Node getParent() { return parent; }

}
