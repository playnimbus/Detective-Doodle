#pragma strict
//Entities
var player:GameObject;
var spawnPoint:Transform;

//Network
var gameName:String = "Join Game: Doodle Detective";

private var refreshing:boolean;
private var hostData:HostData[];
//GUI
var customGuiStyle : GUIStyle;

private var btnX : float;
private var btnY : float;
private var btnW : float;
private var btnH : float;

private var DrawBox : boolean;

//show screen
function Start () 
{
    DrawBox = false;
    btnX = Screen.width * 0.2;
    btnY = Screen.height * 0.05;
    btnW = Screen.width * 0.1;
    btnH = Screen.width * 0.1;
}

function startServer()
{
    Network.InitializeServer(5, 2501, !Network.HavePublicAddress);
    MasterServer.RegisterHost(gameName, "Join Game: Doodle Detective", "Mystery Murder Party Game");
    
}

function refreshHostLists()
{
    MasterServer.RequestHostList(gameName);
    refreshing = true;
    Debug.Log(MasterServer.PollHostList().Length);
}

function Update()
{
    if(refreshing)
    {
        if(MasterServer.PollHostList().Length > 0)
        {
            refreshing = false;
            Debug.Log(MasterServer.PollHostList().Length);
            hostData = MasterServer.PollHostList();
        }
    }
}

function spawnPlayer()
{
}

//Messages
function OnServerInitialized()
{
    Debug.Log("Server Initialized!");
    spawnPlayer();
}

function OnConnectedToServer()
{
    spawnPlayer();
}

function OnMasterServerEvent(mse:MasterServerEvent)
    {
        if(mse == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Registered Server!");
        }
    }

    function OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {

            if (GUI.Button(Rect(20, 50, 200, 40), "Host Game"))
            {
                Debug.Log("Starting Server");
                startServer();
        
                DrawBox = true;
            }
            if (GUI.Button(Rect(20, 100, 200, 40), "Refresh Hosts"))
            {
                Debug.Log("Refreshing");
                refreshHostLists();
            }
        
            if(hostData)
            {
                for (var i:int = 0; i < hostData.length; i++)
                {
                    if (GUI.Button(Rect(btnX * 1.5 + btnW, btnY * 1.2 + (btnH * i), btnW * 3, btnH * .5), hostData[i].gameName))
                    {
                            Network.Connect(hostData[i]);
                            DrawBox = true;
                    }
                }
            }
        }
    if (DrawBox)
    {
        if (GUI.Button(Rect(140, 50, 100, 40), "Spawn"))
        {
            Destroy(GameObject.FindWithTag("MainCamera"));
            Network.Instantiate(player, spawnPoint.position, Quaternion.identity, 0);
            DrawBox = false;
        }
    }
}

    