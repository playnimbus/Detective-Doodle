using UnityEngine;
using System.Collections;


/*
 * CameraController is used to toggle between the different camera angles in the scene. When you switch
 * cameras this script toggles on the desired camera and toggles off the others.
 * 
 * */
public class CameraController : MonoBehaviour {

    public GameObject FirstPersonController;
    public GameObject GuiCamera;
    public GameObject WallCamera;
    public GameObject OverheadCamera;
    public GameObject orthoCamera;

    public GameObject nodeBuilder;

    static public bool playerActive = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("1"))
        {
            activateCamera1();
        }
        if (Input.GetKeyDown("2"))
        {
            activateCamera2();
        }
        if (Input.GetKeyDown("3"))
        {
            activateCamera3();
        }
        if (Input.GetKeyDown("4"))
        {
            activateCamera4();
        }

        if (Input.GetKeyDown("n"))
        {
            toggleNodes();
        }
        if (Input.GetKeyDown("b"))
        {
            toggleAiZones();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(0);
        }	

	}

    public void activateCamera1()
    {
        FirstPersonController.SetActive(true);
        GuiCamera.SetActive(true);
        WallCamera.SetActive(false);
        OverheadCamera.SetActive(false);
        orthoCamera.SetActive(false);
        CameraController.playerActive = true;
    }
    public void activateCamera2()
    {
        FirstPersonController.SetActive(false);
        GuiCamera.SetActive(false);
        WallCamera.SetActive(true);
        OverheadCamera.SetActive(false);
        orthoCamera.SetActive(false);
        CameraController.playerActive = false;
    }
    public void activateCamera3()
    {
        FirstPersonController.SetActive(false);
        GuiCamera.SetActive(false);
        WallCamera.SetActive(false);
        OverheadCamera.SetActive(true);
        orthoCamera.SetActive(false);
        CameraController.playerActive = false;
    }
    public void activateCamera4()
    {
        FirstPersonController.SetActive(false);
        GuiCamera.SetActive(false);
        WallCamera.SetActive(false);
        OverheadCamera.SetActive(false);
        orthoCamera.SetActive(true);
        CameraController.playerActive = false;
    }
    public void toggleNodes()
    {
       // nodeBuilder.GetComponent<NodeBuilder>().ToggleNodesMeshRenderer();
    }
    public void toggleAiZones()
    {
        nodeBuilder.GetComponent<NodeBuilder>().ToggleZonesMeshRenderer();
    }
}
