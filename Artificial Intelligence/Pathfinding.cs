using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * I use the A* pathdinging algorithm. To avoid long paths being found and causing frames to hang, I implemented zones to break up the paths.
 * 
 * the entity first finds a zone path to its destination zone, and will then find a path to the closest node in the next zone on the zone path. once 
 * it enters the next zone on the zone path it finds another path to the next zone until it arrives in the destination zone, then it gets a path for
 * the destination node.
 * 
 * I have two pathfinding methods, one for the nodes and one for the zones. both use A* but have some minor differences.
 */

public class Pathfinding : MonoBehaviour {

    List<Node> nodeList;
    List<Node> nodeCheckedList;

    GameObject[] zoneList;
    List<Zone> zoneCheckedList;

    static public int numPathsThisFrame = 0;

    // Update is called once per frame
    void Update()
    {
        numPathsThisFrame = 0;
    }

    //to avoid Astar looking up really long paths, this function filters and finds the destination zone first to get the path in multiple chunks rather then one long one.
    public List<Node> getPath(GameObject _gameObject, Node _startNode, Node _destinationNode)
    {
        List<Node> path = null;

        nodeList = gameObject.GetComponent<NodeBuilder>().Nodes;

        //if entity is already within destination zone
        if (_startNode.zone == _destinationNode.zone) 
        {
            path = Astar(_startNode, _destinationNode);
        }
        else
        {
            if (_gameObject.GetComponent<CreepController>().zonePath.Count <= 0)
            {
                _gameObject.GetComponent<CreepController>().zonePath = AstarZone(_startNode.zone, _destinationNode.zone);
                _gameObject.GetComponent<CreepController>().zonePath.RemoveAt(0);   //starting zone is not needed
            }

            //find closest node in next zone
            List<Node> nodesInNextZone = _gameObject.GetComponent<CreepController>().zonePath[0].GetComponent<Zone>().NodesInZone;
            Node closestNode = null;
            float lowestDistance = -1;

            for (int i = 0; i < nodesInNextZone.Count; i++)
            {
                if (nodesInNextZone[i].status != Node.Status.closed)
                {
                    if (closestNode == null)
                    {
                        closestNode = nodesInNextZone[i];
                        lowestDistance = findDistance(closestNode.position, _startNode.position);
                    }
                    else
                    {
                        float tempDistance = findDistance(nodesInNextZone[i].position, _startNode.position);
                        if (tempDistance < lowestDistance)
                        {
                            lowestDistance = tempDistance;
                            closestNode = nodesInNextZone[i];
                        }
                    }
                }
            }

            if (closestNode.status == Node.Status.closed)
            {
                print("pathfinding.getpath found bad destination: " + closestNode.name);
            }

            path = Astar(_startNode, closestNode);

        }

        return path;
    }

    public List<Node> Astar(Node _startNode, Node _destinationNode)
    {
        numPathsThisFrame++;

        nodeList = gameObject.GetComponent<NodeBuilder>().Nodes;
        gameObject.GetComponent<NodeBuilder>().resetNodes();
        nodeCheckedList = new List<Node>();

        Node activeNode = _startNode;
        Node destinationNode = _destinationNode;

        activeNode.setCostFromStart(0);
        activeNode.setCostToDestination(findDistance(activeNode.position, destinationNode.position));
        activeNode.setTotalCost(activeNode.getCostFromStart() + activeNode.getCostToDestination());
        activeNode.setParent(null);

        Node connectionBeingChecked = null;

        while (activeNode.name != destinationNode.name)
        {
            activeNode.setStatus(Node.Status.closed);

            //erase new active node from checkedList
            for (int i = 0; i < nodeCheckedList.Count; i++)
            {
                if (nodeCheckedList[i].name == activeNode.name)
                {
                    nodeCheckedList.RemoveAt(i);
                    break;
                }
            }

            if (AnyAvailableConnections(activeNode) == false)  //if it is trapped then reroute it to the cheapest node that has been checked but not closed
            {
                Node cheapestNode = null;
                for (int i = 0; i < nodeCheckedList.Count; i++)
                {
                    if (nodeCheckedList[i].status != Node.Status.closed)
                    {
                        if (AnyAvailableConnections(nodeCheckedList[i]) == true)
                        {
                            if (cheapestNode == null)
                            {
                                cheapestNode = nodeCheckedList[i];
                            }
                            else if (nodeCheckedList[i].totalCost < cheapestNode.totalCost)
                            {
                                cheapestNode = nodeCheckedList[i];
                            }
                        }
                    }
                }

                if (activeNode.name == destinationNode.name)
                {
                    break;
                }
                
                activeNode = cheapestNode;
                activeNode.setStatus(Node.Status.closed);
            }

            float activeNodesCheapestCost = -1;
            Node activeNodesCheapestConnection = null;

            for (int i = 0; i < activeNode.numConnections; i++)
            {
                connectionBeingChecked = activeNode.Connections[i];

                if (connectionBeingChecked.getStatus() == Node.Status.open)
                {
                    connectionBeingChecked.setCostFromStart(activeNode.getCostFromStart() + findDistance(activeNode.position, connectionBeingChecked.position));
                    connectionBeingChecked.setCostToDestination(findDistance(connectionBeingChecked.position, destinationNode.position));
                    connectionBeingChecked.setTotalCost(connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination());
                    connectionBeingChecked.setParent(activeNode);
                    connectionBeingChecked.setStatus(Node.Status.check);

                    nodeCheckedList.Add(connectionBeingChecked);

                    //track the cheapest connection for the current activeNode
                    if (connectionBeingChecked == destinationNode)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                        break;
                    }
                    else if (activeNodesCheapestCost == -1)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                    }
                    else if (connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination() < activeNodesCheapestCost)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                    }
                }

                else if (connectionBeingChecked.getStatus() == Node.Status.check)
                {
                    //check if the already checked connection would be a cheaper path then the connection that is already selected
                    if (connectionBeingChecked.getTotalCost() > activeNode.getTotalCost() + findDistance(connectionBeingChecked.position, activeNode.position))
                    {
                        connectionBeingChecked.setParent(activeNode);
                        connectionBeingChecked.setCostFromStart(activeNode.getTotalCost() + findDistance(connectionBeingChecked.position, activeNode.position));
                        connectionBeingChecked.setCostToDestination(findDistance(connectionBeingChecked.position, destinationNode.position));
                        connectionBeingChecked.setTotalCost(connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination());
                    }

                    //track the cheapest connection for the current activeNode
                    if (connectionBeingChecked == destinationNode)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                        break;
                    }
                    else if (activeNodesCheapestCost == -1)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                    }
                    else if (connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination() < activeNodesCheapestCost)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                        activeNodesCheapestConnection = connectionBeingChecked;
                    }
                }
            }

            if (activeNodesCheapestConnection == destinationNode)
            {
                activeNode = activeNodesCheapestConnection;
            }
            else if (AnyAvailableConnections(activeNodesCheapestConnection) == false)
            {
                activeNodesCheapestCost = -1;
                Node findNextActive = null;

                for (int i = 0; i < nodeCheckedList.Count; i++)
                {
                    if (nodeCheckedList[i].name != activeNode.name)
                    {
                        if (nodeCheckedList[i].getStatus() == Node.Status.check)
                        {
                            if (activeNodesCheapestCost == -1)
                            {
                                activeNodesCheapestCost = nodeCheckedList[i].getTotalCost();
                                findNextActive = nodeCheckedList[i];
                            }

                            else if (activeNodesCheapestCost >= nodeCheckedList[i].getTotalCost())
                            {
                                activeNodesCheapestCost = nodeCheckedList[i].getTotalCost();
                                findNextActive = nodeCheckedList[i];
                            }
                        }
                    }
                }
                activeNode = findNextActive;
            }
            else
            {
                activeNode = activeNodesCheapestConnection;
            }
        }
        List<Node> pathReverse = new List<Node>();

        while (activeNode != null)
        {
            pathReverse.Add(activeNode);
            Node parent = activeNode.getParent();
            if (parent == null)
            {
                break;
            }
            activeNode = parent;
        }
        List<Node> path = new List<Node>();
        for (int i = 0; i < pathReverse.Count; i++)
        {
            path.Add(pathReverse[(pathReverse.Count - 1) - i]);
        }
        gameObject.GetComponent<NodeBuilder>().resetNodes();
        nodeCheckedList.Clear();

        return path;
    }

    public List<GameObject> AstarZone(GameObject _startZone, GameObject _destinationZone)
    {
        numPathsThisFrame++;

        zoneList = gameObject.GetComponent<NodeBuilder>().Zones;
        gameObject.GetComponent<NodeBuilder>().resetZones();
        zoneCheckedList = new List<Zone>();

        Zone activeZone = _startZone.GetComponent<Zone>();
        Zone destinationZone = _destinationZone.GetComponent<Zone>();

        activeZone.setCostFromStart(0);
        activeZone.setCostToDestination(findDistance(activeZone.gameObject.transform.position, destinationZone.gameObject.transform.position));
        activeZone.setTotalCost(activeZone.getCostFromStart() + activeZone.getCostToDestination());
        activeZone.setParent(null);

        Zone connectionBeingChecked = null;

        while (activeZone.gameObject.name != destinationZone.gameObject.name)
        {
            activeZone.setStatus(Zone.Status.closed);

            //erase new active node from checkedList
            for (int i = 0; i < zoneCheckedList.Count - 1; i++)
            {
                if (zoneCheckedList[i].name == activeZone.name)
                {
                    zoneCheckedList.RemoveAt(i);
                    i = zoneCheckedList.Count + 10; //break loop
                }
            }

            if (AnyAvailableConnections(activeZone) == false)  //if it is trapped then reroute it to the cheapest node that has been checked but not closed
            {
                Zone cheapestZone = null;
                for (int i = 0; i < zoneCheckedList.Count; i++)
                {
                    if (zoneCheckedList[i].status != Zone.Status.closed)
                    {
                        if (AnyAvailableConnections(zoneCheckedList[i]) == true)
                        {
                            if (cheapestZone == null)
                            {
                                cheapestZone = zoneCheckedList[i];
                            }
                            else if (zoneCheckedList[i].totalCost < cheapestZone.totalCost)
                            {
                                cheapestZone = zoneCheckedList[i];
                            }
                        }
                    }
                }

                if (activeZone == destinationZone)
                {
                    break;
                }
                activeZone = cheapestZone;
                activeZone.setStatus(Zone.Status.closed);
            }

            float activeNodesCheapestCost = 0;

            for (int i = 0; i < activeZone.Connections.Length; i++)
            {
                connectionBeingChecked = activeZone.Connections[i].GetComponent<Zone>();

                if (connectionBeingChecked.getStatus() == Zone.Status.open)
                {
                    connectionBeingChecked.setCostFromStart(activeZone.getCostFromStart() + findDistance(activeZone.gameObject.transform.position, connectionBeingChecked.gameObject.transform.position));
                    connectionBeingChecked.setCostToDestination(findDistance(connectionBeingChecked.gameObject.transform.position, destinationZone.gameObject.transform.position));
                    connectionBeingChecked.setTotalCost(connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination());
                    connectionBeingChecked.setParent(activeZone.gameObject);
                    connectionBeingChecked.setStatus(Zone.Status.check);

                    zoneCheckedList.Add(connectionBeingChecked);

                    //track the cheapest connection for the current activeNode
                    if (connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination() < activeNodesCheapestCost)
                    {
                        activeNodesCheapestCost = connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination();
                    }
                }

                else if (connectionBeingChecked.getStatus() == Zone.Status.check)
                {
                    //check if the already checked connection would be a cheaper path then the connection that is already selected
                    if (connectionBeingChecked.getTotalCost() > activeZone.getTotalCost() + findDistance(connectionBeingChecked.gameObject.transform.position, activeZone.gameObject.transform.position))
                    {
                        connectionBeingChecked.setParent(activeZone.gameObject);
                        connectionBeingChecked.setCostFromStart(activeZone.getTotalCost() + findDistance(connectionBeingChecked.gameObject.transform.position, activeZone.gameObject.transform.position));
                        connectionBeingChecked.setCostToDestination(findDistance(connectionBeingChecked.gameObject.transform.position, destinationZone.gameObject.transform.position));
                        connectionBeingChecked.setTotalCost(connectionBeingChecked.getCostFromStart() + connectionBeingChecked.getCostToDestination());
                    }
                }
            }

            activeNodesCheapestCost = -1;
            Zone findNextActive = null;

            for (int i = 0; i < zoneCheckedList.Count; i++)
            {
                if (zoneCheckedList[i].gameObject.name != activeZone.gameObject.name)
                {
                    if (zoneCheckedList[i].getStatus() == Zone.Status.check)
                    {
                        if (activeNodesCheapestCost == -1)
                        {
                            activeNodesCheapestCost = zoneCheckedList[i].getTotalCost();
                            findNextActive = zoneCheckedList[i];
                        }

                        else if (activeNodesCheapestCost >= zoneCheckedList[i].getTotalCost())
                        {
                            activeNodesCheapestCost = zoneCheckedList[i].getTotalCost();
                            findNextActive = zoneCheckedList[i];
                        }
                    }
                }
            }
            activeZone = findNextActive;

        }
        List<GameObject> pathReverse = new List<GameObject>();

        while (activeZone != null)
        {
            pathReverse.Add(activeZone.gameObject);
            GameObject parent = activeZone.getParent();
            if (parent == null)
            {
                break;
            }
            activeZone = parent.GetComponent<Zone>();
        }
        List<GameObject> path = new List<GameObject>();
        for (int i = 0; i < pathReverse.Count; i++)
        {
            path.Add(pathReverse[(pathReverse.Count - 1) - i]);
        }
        gameObject.GetComponent<NodeBuilder>().resetZones();
        zoneCheckedList.Clear();

        return path;
    }

    float findDistance(Vector3 node1, Vector3 node2)
    {
        return Vector3.Distance(node1, node2);
    }

    //returns true if the node passed in has open connections.
    //returns false if all connections are closed
    bool AnyAvailableConnections(Node _node)
    {
        for (int i = 0; i < _node.numConnections; i++)
        {
            Node tempConnection = _node.Connections[i];

            if (tempConnection.getStatus() == Node.Status.check ||
                tempConnection.getStatus() == Node.Status.open)
            {
                return true;
            }
        }
        return false;
    }
    //returns true if the zone passed in has open connections.
    //returns false if all connections are closed
    bool AnyAvailableConnections(Zone _zone)
    {
        for (int i = 0; i < _zone.Connections.Length; i++)
        {
            Zone tempConnection = _zone.Connections[i].GetComponent<Zone>();

            if (tempConnection.getStatus() == Zone.Status.check ||
                tempConnection.getStatus() == Zone.Status.open)
            {
                return true;
            }
        }
        return false;
    }
}