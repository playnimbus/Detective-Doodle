using UnityEngine;
using System.Collections;

public class LootMicroGame : MonoBehaviour {

    public static bool lootingEnabled = false;      //allows other scripts to know if looting is active. primatily PlayerMovement script

    public Camera camera;
    public GameObject drawer;

    Vector3 OriginalDrawerPosition;
    Vector3 AvailableOffset = new Vector3(0,0,-5);
    Vector3 OpenOffset = new Vector3(0,0,-24);

    public enum DrawerStates { hidden, opening , available, open };
    public DrawerStates state = DrawerStates.hidden;

    float lerpTime = 0.15f;
    float currentLerpTime = 0;
    Vector3 startPos;
    Vector3 endPos;

	// Use this for initialization
	void Start () {
        OriginalDrawerPosition = drawer.transform.position;
       // SwitchState(DrawerStates.available);      //uncomment to start with drawer available, good for debugging the motions and states of the drawer
	}
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case DrawerStates.hidden: UpdateHidden(); break;
            case DrawerStates.available: UpdateAvailable(); break;
            case DrawerStates.opening: UpdateOpening(); break;
            case DrawerStates.open: UpdateOpen(); break;
        }
	}

    public void MakeDrawerAvailable()     //init microgame objects here
    {
        SwitchState(DrawerStates.available);
    }

    public void MakeDrawerHidden()      //dispose of microgame objects here
    {
        SwitchState(DrawerStates.hidden);
    }

    void SwitchState(DrawerStates newState)
    {
        switch (state)
        {
            case DrawerStates.hidden: EndHidden(); break;
            case DrawerStates.available: EndAvailable(); break;
            case DrawerStates.opening: EndOpening(); break;
            case DrawerStates.open: EndOpen(); break;
        }
        state = newState;

        switch (state)
        {
            case DrawerStates.hidden: BeginHidden(); break;
            case DrawerStates.available: BeginAvailable(); break;
            case DrawerStates.opening: BeginOpening(); break;
            case DrawerStates.open: BeginOpen(); break;
        }
    }

    #region Hidden State
    void BeginHidden()
    {
        LootMicroGame.lootingEnabled = false;
        startPos = drawer.transform.position;
        endPos = OriginalDrawerPosition;
        currentLerpTime = 0;
    }

    void UpdateHidden()
    {
        if (drawer.transform.position.z != OriginalDrawerPosition.z)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            drawer.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(startPos, endPos, perc));
        }
    }

    void EndHidden()
    {
    }
    #endregion

    //State of the draw showing at the top of your screen but hasn't yet been pulled down
    #region Avaialable State

    void BeginAvailable()
    {
        LootMicroGame.lootingEnabled = false;
        startPos = drawer.transform.position;
        endPos = OriginalDrawerPosition + AvailableOffset;
        currentLerpTime = 0;
    }

    void UpdateAvailable() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "LootDrawer")
                {
                    SwitchState(DrawerStates.opening);
                }
            }
        }

        if (drawer.transform.position.z != OriginalDrawerPosition.z + AvailableOffset.z)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            drawer.GetComponent<Rigidbody>().MovePosition( Vector3.Lerp(startPos, endPos, perc));
        }
    }

    void EndAvailable()
    {

    }

    #endregion

    //State while the player is currently dragging the drawer down the screen.
    //if it is released above half way down the screen then it goes back to available.
    //if it is released below half way down the screen then it goes to open state.
    #region Opening State

    void BeginOpening()
    {
        LootMicroGame.lootingEnabled = true;
    }

    void UpdateOpening()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit))
            {                                                                                                                                   //+5 is a manual offset for the center of the drawer gameobject
                drawer.GetComponent<Rigidbody>().MovePosition( new Vector3(drawer.transform.position.x, drawer.transform.position.y, hit.point.z +5));
            }
        }
        else
        {    
            if (drawer.transform.position.z < OriginalDrawerPosition.z + (OpenOffset.z/2))
            {
                SwitchState(DrawerStates.open);
            }
            else
            {
                SwitchState(DrawerStates.available);
            }
        }
    }

    void EndOpening()
    {

    }

    #endregion

    //When the drawwer is fully opened.
    #region Open State
    void BeginOpen()
    {
        LootMicroGame.lootingEnabled = true;
        startPos = drawer.transform.position;
        endPos = OriginalDrawerPosition + OpenOffset;
        currentLerpTime = 0;
    }

    void UpdateOpen()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "LootDrawer")
                {
                    SwitchState(DrawerStates.opening);
                }
            }
        }

        if (drawer.transform.position.z != OriginalDrawerPosition.z + OpenOffset.z)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            drawer.GetComponent<Rigidbody>().MovePosition( Vector3.Lerp(startPos, endPos, perc));
        }
    }

    void EndOpen()
    {

    }
    #endregion
}
