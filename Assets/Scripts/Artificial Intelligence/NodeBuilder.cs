using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * NodeBuilder builds the grid of nodes and handles some of the higher level management of the node grid.
 */ 

public class NodeBuilder : MonoBehaviour {

    public List<Node> Nodes;
    public List<Node> EdgeNodes;
    List<GameObject> TempNodes;
    public GameObject[] Zones;
    public bool drawNodes = false;
    public bool drawConnections = false;
    int numNodes = 0;
	// Use this for initialization

    public bool nodesConnectionsDirty = false;
    public bool nodesDeletedThisFrame = false;

    void Awake()
    {
        buildNodes();   //builds nodes
        getAllNodeConnections();    //finds the nodes connections

        GameObject[] levelGeometry = GameObject.FindGameObjectsWithTag("LevelGeometry");    //finds all the level geometry

        //move colliders up for node spherecasts
        for (int i = 0; i < levelGeometry.Length; i++)
        {
            if (levelGeometry[i].transform.localScale.y < 1)
            {
                levelGeometry[i].GetComponent<BoxCollider>().center = new Vector3(0, 2f, 0);
            }
            else if (levelGeometry[i].transform.localScale.y > 10)
            {
                levelGeometry[i].GetComponent<BoxCollider>().size = new Vector3(1f, 0.8f, 1f);
                levelGeometry[i].GetComponent<BoxCollider>().center = new Vector3(0, 0.2f, 0);
            }
            else
            {
                levelGeometry[i].GetComponent<BoxCollider>().size = new Vector3(1f, 0.5f, 1f);
                levelGeometry[i].GetComponent<BoxCollider>().center = new Vector3(0, 0.25f, 0);
            }
        }

        checkAllNodesForLevelGeometry();    //calls the method in every node to check for geometry

        //move colliders back into place
        for (int i = 0; i < levelGeometry.Length; i++)
        {
            levelGeometry[i].GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 1f);
            levelGeometry[i].GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
        }        
    }

	void Start () {

	}

    //buildNodes uses the bounds of the zone colliders to determine where to build nodes. it also accounts for the rotation value of the zone
    //as well for ramps or stairs.
    public void buildNodes()
    {
        Zones = GameObject.FindGameObjectsWithTag("Zone");
        TempNodes = new List<GameObject>();
        eraseNodes();

        Vector3 bounds;

        for (int i = 0; i < Zones.Length; i++)
        {
            Quaternion rotation = Zones[i].transform.rotation;
            Zones[i].transform.rotation = Quaternion.EulerRotation(0, 0, 0);

            bounds = Zones[i].GetComponent<BoxCollider>().bounds.size;
            for (int x = 0; x < bounds.x; x++)
            {
                for (int y = 0; y < bounds.z; y++)
                {
                     spawnNode(new Vector3((Zones[i].transform.position.x + x) - (bounds.x / 2), Zones[i].transform.position.y, (Zones[i].transform.position.z + y) - (bounds.z / 2)), i);
                }
            }
            Zones[i].transform.parent = gameObject.transform;
            Zones[i].transform.rotation = rotation;
        }

        //set positions of SO Nodes
        for (int i = Nodes.Count-1; i >= 0; i--)
        {
            Nodes[i].position = TempNodes[i].transform.position;
            GameObject.DestroyImmediate(TempNodes[i]);
        }
        TempNodes.Clear();
    }

    //called from editor script to erase nodes from scene file.
    public void eraseNodes()
    {
        //clear all the nodes on screen
        for (int i = 0; i < Nodes.Count; i++)
        {
            GameObject.DestroyImmediate(Nodes[i]);

        }
        Nodes.Clear();

        for (int i = 0; i < Zones.Length; i++)
        {
            Zones[i].GetComponent<Zone>().NodesInZone.Clear();
        }
    }

    //spawnNode creates a GameObject Node and a ScriptableObject Node. GameObject Node is used to set the position from the zone initialization
    // and is then copied into the scriptible object node and then deleted at the start of the scene.
    void spawnNode(Vector3 pos, int zoneID)
    {
        Node node = ScriptableObject.CreateInstance("Node") as Node;

        GameObject tempNode = new GameObject();
        tempNode.transform.parent = Zones[zoneID].transform;
        tempNode.transform.position = new Vector3(pos.x, pos.y, pos.z);
        TempNodes.Add(tempNode);

        node.name = "Node_" + numNodes;
        node.zone = Zones[zoneID];
        node.zone.GetComponent<Zone>().NodesInZone.Add(node);
        Nodes.Add(node);
        numNodes++;
    }

    //debugging nodes
    //draws the nodes in the editor view using gizmos. activated when the public bool is toggled from the inspector.
    void OnDrawGizmos()
    {
        if (drawNodes == true)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Gizmos.color = Nodes[i].gizmoColor;
                Gizmos.DrawCube(Nodes[i].position, new Vector3(0.25f, 0.25f, 0.25f));

                if (drawConnections == true)
                {
                    for (int j = 0; j < Nodes[i].numConnections; j++)
                    {
                        Gizmos.DrawLine(Nodes[i].position, Nodes[i].Connections[j].position);
                    }
                }
            }
        }
    }

    public void getAllNodeConnections()
    {
        for (int i = 0; i < Zones.Length; i++)  //builds connections within zone
        {
            EdgeNodes.AddRange( Zones[i].GetComponent<Zone>().buildNodeConnections());
        }

        for (int i = 0; i < EdgeNodes.Count; i++) //build zone connecting nodes
      {
          EdgeNodes[i].getNodesConnections();
      }

        nodesConnectionsDirty = false;
    }

    public void checkAllNodesForLevelGeometry()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].checkForLevelGeometry();
        }
    }

    public void resetNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].resetNode();
        }
    }
    public void resetZones()
    {
        for (int i = 0; i < Zones.Length; i++)
        {
            Zones[i].GetComponent<Zone>().resetZone();
        }
    }

    public void ToggleZonesMeshRenderer()
    {
        for (int i = 0; i < Zones.Length; i++)
        {
            Zones[i].GetComponent<MeshRenderer>().enabled = !Zones[i].GetComponent<MeshRenderer>().enabled;
        }
    }

    public void deleteNode(string _name)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].name == _name)
            {
                Nodes.RemoveAt(i);
                break;
            }
        }
    }
}
