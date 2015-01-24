using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* CreepController is the main brains of the enemy AI, All of their behavior is controlled here.
 * 
 * Enemy entities use a Finite State Machine to determine their actions. currently these states are supported:
 * -Idle: Entity waits for next command
 * -walkPath: Finds a path to a destination and begins walking it
 * -attack: When in range from the player the enemy will switch to its attack state and begin attacking.
 * -pursue: When in range entity will break from path and chase towards player.
 *          during pursuit line of sight must be maintained to player or entity will stop pursuit after an amount of time without vision.
 *          Entity will path around wall while pursueing if required.
 * -Death: when entities health reaches 0, the entity plays its death animation and then is removed from the scene.
 * 
 */

public class CreepController : MonoBehaviour {

    public float walkSpeed = 7;
    public int health = 10;
    int attackDamage = 1;
    float attackRange = 3;

    GameObject nodeBuilder;
    public List<Node> path = null;
    public List<GameObject> zonePath = null;
    public Node NextNodeOnPath = null;
    public Node DestinationNode = null;
    public Node currentNode = null;

    GameObject Player;
    public GameObject creepRenderer;
    public GameObject alertMark;

    public enum enemyStates { idle, walkPath, attack, death, pursue };
    public enemyStates enemyState;

    AnimationState idleAnimState;
    AnimationState walkAnimState;

    AnimationState attackAnimState;
    float attackSpeed = 3;
    float attackTimer = 0;
    public GameObject hitEffect;
    public float animationOffset;

    AnimationState deathAnimState;

    public Node destinationNodeDebug;

	// Use this for initialization
	void Start () {

        gameObject.rigidbody.AddForce(new Vector3(1, 0, 0));

        nodeBuilder = GameObject.Find("NodeBuilder");

        /*
        foreach (AnimationState state in animation)
        {
            switch (state.name)
            {
                case "idle": idleAnimState = state; break;
                case "walk": walkAnimState = state; break;
                case "attack": attackAnimState = state; break;
                case "death": deathAnimState = state; break;
            }
        }
        */
        idleStateEnter();
        enemyState = enemyStates.idle;
	}
	
	// Update is called once per frame

	void Update () {

        checkCurrentNode();

        switch (enemyState)
        {
            case enemyStates.idle: idleStateUpdate(); break;
            case enemyStates.walkPath: walkStateUpdate(); break;
            case enemyStates.attack: attackStateUpdate(); break;
            case enemyStates.death: deathStateUpdate(); break;
            case enemyStates.pursue: pursueStateUpdate(); break;
        }

        /*
        if (enemyState == enemyStates.walkPath || enemyState == enemyStates.idle)
        {
            if (Vector3.Distance(Player.transform.position, gameObject.transform.position) <= 15)
            {
                RaycastHit hit;
                Vector3 directionToPlayer = ((Player.transform.position - (new Vector3(0, Player.GetComponentInChildren<MeshRenderer>().bounds.size.y / 2, 0))) - gameObject.transform.position).normalized;
                if (Physics.Raycast(gameObject.transform.position + (new Vector3(0, gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y / 2, 0)), directionToPlayer, out hit))
                {
                    if (hit.transform.tag == "Player")
                    {
                        changeEnemyState(enemyStates.pursue);
                    }
                }
            }
        }
            * */
        
	}

    void changeEnemyState(enemyStates newState)
    {
        switch (enemyState)
        {
            case enemyStates.idle: idleStateExit(); break;
            case enemyStates.walkPath: walkStateExit(); break;
            case enemyStates.attack: attackStateExit(); break;
            case enemyStates.death: deathStateExit(); break;
            case enemyStates.pursue: pursueStateExit(); break;
        }

        enemyState = newState;

        switch (enemyState)
        {
            case enemyStates.idle: idleStateEnter(); break;
            case enemyStates.walkPath: walkStateEnter(); break;
            case enemyStates.attack: attackStateEnter(); break;
            case enemyStates.death: deathStateEnter(); break;
            case enemyStates.pursue: pursueStateEnter(); break;
        }
    }

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

    //-------------- PURSUE STATE -----------------------
    float sightTimer = 0;
    void pursueStateEnter()
    {
   //     animation.Play("run");
        DestinationNode = null;
        sightTimer = 0;
        alertMark.SetActive(true);
    }

    float pursueWallTimer = 0;
    string oldCurrentNode = "";
    public Vector3 directionDebug;
    void pursueStateUpdate()
    {
        /*
        if (animation.isPlaying == false)
        {
            animation.Play("run");
        }
         * */
        if (currentNode.name == oldCurrentNode)
        {
            pursueWallTimer += Time.deltaTime;
            if (pursueWallTimer >= 1)
            {
                DestinationNode = Player.GetComponent<Player>().currentNode;
                changeEnemyState(enemyStates.walkPath);
            }
        }
        else
        {
            pursueWallTimer = 0;
        }


        alertMark.transform.LookAt(Player.transform.position);
        gameObject.transform.LookAt(new Vector3(Player.transform.position.x, gameObject.transform.position.y, Player.transform.position.z));

        Vector3 directionToDesitination = (Player.transform.position - gameObject.transform.position).normalized;
        directionToDesitination = (directionToDesitination * walkSpeed) * Time.deltaTime;

        gameObject.transform.position += new Vector3(directionToDesitination.x, 0, directionToDesitination.z);


        if (Vector3.Distance(Player.transform.position, gameObject.transform.position) <= 15)
        {
            RaycastHit hit;
            if (Physics.Raycast(gameObject.transform.position + (new Vector3(0, gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y / 2, 0)), ((Player.transform.position - (new Vector3(0, Player.GetComponentInChildren<MeshRenderer>().bounds.size.y / 2, 0))) - gameObject.transform.position).normalized, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    sightTimer = 0;
                }
                else if (hit.transform.tag == "LevelGeometry")
                {
                    sightTimer += Time.deltaTime;

                    if (Player.GetComponent<Player>().currentNode.status == Node.Status.closed)
                    {
                        destinationNodeDebug = Player.GetComponent<Player>().currentNode;
                        print("Entity sent bad destination");
                    }
                    DestinationNode = Player.GetComponent<Player>().currentNode;
                    changeEnemyState(enemyStates.walkPath);
                }
                else if (hit.transform.tag != "Player")
                {
                    sightTimer += Time.deltaTime;
                    if (sightTimer >= 2)
                    {
                        changeEnemyState(enemyStates.idle);
                    }
                }
            }
            else
            {
                changeEnemyState(enemyStates.idle);
            }
        }
        else
        {
            changeEnemyState(enemyStates.idle);
        }

        oldCurrentNode = currentNode.name;
    }
    void pursueStateExit()
    {
        alertMark.SetActive(false);
    }

    //-------------- walkPath STATE -----------------------

    void walkStateEnter()
    {
    //    animation.Play("run");

        if (DestinationNode != null)
        {
            getPath(DestinationNode);
        }
        else
        {
            getPath();
        }

        NextNodeOnPath = path[1];
    }
    void walkStateUpdate()
    {
        /*
        if (animation.isPlaying == false)
        {
            animation.Play("run");
        }
         * */
        if (NextNodeOnPath != null)
        {
            gameObject.transform.LookAt(new Vector3(NextNodeOnPath.position.x, gameObject.transform.position.y, NextNodeOnPath.position.z));
        }

        Vector3 directionToDesitination = (NextNodeOnPath.position - gameObject.transform.position).normalized;
        directionToDesitination = (directionToDesitination * walkSpeed) * Time.deltaTime;
        gameObject.transform.position += new Vector3(directionToDesitination.x, 0, directionToDesitination.z);

        if (Vector3.Distance(gameObject.transform.position, NextNodeOnPath.position) <= 0.7f)
        {
            if (NextNodeOnPath.zone == DestinationNode.zone)
            {
                if (NextNodeOnPath == DestinationNode)        //if final destination is reached
                {
                    path.RemoveAt(0);
                    NextNodeOnPath = null;
                    DestinationNode = null;
                    changeEnemyState(enemyStates.idle);
                    return;
                }

                else if (path[path.Count-1] != DestinationNode)                //if zone has been entered but final destination has not been reached.
                {
                    if (DestinationNode.status == Node.Status.closed)
                    {
                        destinationNodeDebug = DestinationNode;
                        print("Entity sent bad destination1: " + DestinationNode.name);
                    }
                    setCurrentNode(NextNodeOnPath);
                    getPath(DestinationNode);
                    NextNodeOnPath = path[0];
                }
                else 
                {//step forward on path
                    setCurrentNode(path[0]);
                    path.RemoveAt(0);
                    NextNodeOnPath = path[0];
                }

            }
            else if (zonePath.Count > 0)
            {
                if (NextNodeOnPath.zone == zonePath[0])
                {
                    zonePath.RemoveAt(0);

                    if (DestinationNode.status == Node.Status.closed)
                    {
                        destinationNodeDebug = DestinationNode;
                        print("Entity sent bad destination2: " + DestinationNode.name);
                    }
                    setCurrentNode(NextNodeOnPath);
                    getPath(DestinationNode);
                    NextNodeOnPath = path[0];
                }
                else
                {//step forward on path
                    setCurrentNode(path[0]);
                    path.RemoveAt(0);
                    NextNodeOnPath = path[0];
                }
            }
            else
            {//step forward on path
                setCurrentNode(path[0]);
                path.RemoveAt(0);
                NextNodeOnPath = path[0];
            }
    }

    }
    void walkStateExit()
    {
        zonePath.Clear();
        path.Clear();
        NextNodeOnPath = null;
    }

//-------------- IDLE STATE -----------------------
    float idleTimer = 0;
    void idleStateEnter() 
    {
    //    animation.Play("idle");
        DestinationNode = null;
    }
    void idleStateUpdate() 
    {
        idleTimer += Time.deltaTime;
        /*
        if (animation.isPlaying == false)
        {
            animation.Play("idle");
        }
        */
        if (idleTimer > 2 && Pathfinding.numPathsThisFrame < 1)
        {
            if (currentNode != null)
            {
                idleTimer = 0;
                changeEnemyState(enemyStates.walkPath);
            }
        }
       
    }
    void idleStateExit() { }

//-------------- ATTACK STATE -----------------------
    void attackStateEnter() 
    {
     //   animation.Play("attack");
        gameObject.rigidbody.isKinematic = true;
        attackTimer = 0;
        StartCoroutine(CheckAttackRange());

    }
    void attackStateUpdate()
    {
        if (animation.isPlaying == false)
        {
            changeEnemyState(enemyStates.pursue);
        }
    }
    void attackStateExit() 
    {
        gameObject.rigidbody.isKinematic = false;
    }

    float timeUntilHit = 0.5f;
    IEnumerator CheckAttackRange()
    {
        yield return new WaitForSeconds(timeUntilHit);

        if (Vector3.Distance(gameObject.transform.position, Player.transform.position) <= attackRange)
        {
            if (Player.tag == "Player")
            {
                Player.GetComponent<Player>().applyDamage(attackDamage);
            }
        }
    }


    //-------------- DEATH STATE -----------------------
    void deathStateEnter() 
    {
   //     animation.Play("die");
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.rigidbody.isKinematic = true;
    }
    void deathStateUpdate() 
    {
        /*
        if (animation.isPlaying == false)
        {
            DestroyObject(gameObject);
        }
         * */
    }
    void deathStateExit() { }



    //-------------------------------------------------
    bool firstPath = true;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            changeEnemyState(enemyStates.attack);
        }
    }

    public void setCurrentNode(Node _node)
    {
        currentNode = _node;
    }

    void getPath()
    {      
        do
        {
            DestinationNode = nodeBuilder.GetComponent<NodeBuilder>().Nodes[
            Random.Range(0, nodeBuilder.GetComponent<NodeBuilder>().Nodes.Count - 1)];
        } while (DestinationNode.status == Node.Status.closed);

        if (DestinationNode.status == Node.Status.closed)
        {
            print("Entity sent bad destination");
        }

        path = nodeBuilder.GetComponent<Pathfinding>().getPath(gameObject, currentNode, DestinationNode);

        if (path == null)
        {
            Debug.Log("BAD PATH");
            Debug.Break();
        }

    }
    void getPath(Node _destinationNode)
    {
        DestinationNode = _destinationNode;

        path = nodeBuilder.GetComponent<Pathfinding>().getPath(gameObject, currentNode, DestinationNode);

        if (path == null)
        {
            Debug.Log("BAD PATH");
            Debug.Break();
        }
    }

    public void applyDamage(int value)
    {
        health -= value;
        creepRenderer.renderer.materials[0].color = Color.red;
        creepRenderer.renderer.materials[1].color = Color.red;
        StartCoroutine(ChangeHitColorBack(0.1f));

        if (health <= 0)
        {
            GameObject.Find("CreepSpawner").GetComponent<CreepSpawner>().minusCreep();
            changeEnemyState(enemyStates.death);
        }
    }

    IEnumerator ChangeHitColorBack(float sec)
    {
        yield return new WaitForSeconds(sec);
        creepRenderer.renderer.materials[0].color = Color.white;
        creepRenderer.renderer.materials[1].color = Color.white;
    }

}