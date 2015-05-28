using UnityEngine;
using System.Collections;

public class LockedDoorUI : MonoBehaviour {

    public Camera camera;
    public GameObject door;

    Vector3 OriginalDoorPosition;
    Vector3 AvailableOffset = new Vector3(0, 0, -5);

    public enum DoorStates { hidden, available };
    public DoorStates state = DoorStates.hidden;

    float lerpTime = 0.15f;
    float currentLerpTime = 0;
    Vector3 startPos;
    Vector3 endPos;

    Door currentDoor;
    Player currentPlayer;

	// Use this for initialization
	void Start () {
        OriginalDoorPosition = door.transform.position;

      //  SwitchState(DoorStates.available);  
	}

    void Update()
    {
        switch (state)
        {
            case DoorStates.hidden: UpdateHidden(); break;
            case DoorStates.available: UpdateAvailable(); break;
        }
    }

    void SwitchState(DoorStates newState)
    {
        switch (state)
        {
            case DoorStates.hidden: EndHidden(); break;
            case DoorStates.available: EndAvailable(); break;

        }
        state = newState;

        switch (state)
        {
            case DoorStates.hidden: BeginHidden(); break;
            case DoorStates.available: BeginAvailable(); break;

        }
    }

    public void MakeDoorAvailable(Door door, Player player)     //init microgame objects here
    {
        SwitchState(DoorStates.available);

        currentDoor = door;
        currentPlayer = player;
        currentLerpTime = 0;
    }

    public void MakeDoorHidden()      //dispose of microgame objects here
    {
        SwitchState(DoorStates.hidden);

        currentDoor = null;
        currentPlayer = null;
    }
    
    #region Hidden State
    void BeginHidden()
    {
        startPos = door.transform.position;
        endPos = OriginalDoorPosition;
        currentLerpTime = 0;
    }

    void UpdateHidden()
    {
        if (door.transform.position.z != OriginalDoorPosition.z)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            door.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(startPos, endPos, perc));
        }
    }
    void EndHidden()
    {
    }
    #endregion


    #region Avaialable State

    void BeginAvailable()
    {
        startPos = door.transform.position;
        endPos = OriginalDoorPosition + AvailableOffset;
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
                if (hit.collider.tag == "LockedDoor")
                {
                    //in whodunnit murder can always open door regardless of key. this should be moved
                    //into whodunnitPlayer at some point.
                    PlayerWhodunnit whodunnitPlayer = currentPlayer.GetComponent<PlayerWhodunnit>();
/*
                    if (currentPlayer.inventory.ItemInHand == ItemPickups.Key)
                    {
                        currentPlayer.inventory.removeItem();
                        currentDoor.openDoor();
                        SwitchState(DoorStates.hidden);
                    }
 * */
                    //else if
                     if (whodunnitPlayer != null){  //above comment applies here too
                        if (whodunnitPlayer.IsMurderer)
                        {
                            currentDoor.openDoor();
                            SwitchState(DoorStates.hidden);
                        }
                    }
                }
            }
        }

        if (door.transform.position.z != OriginalDoorPosition.z + AvailableOffset.z)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            door.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(startPos, endPos, perc));
        }

    }
    void EndAvailable()
    {

    }

    #endregion

}
