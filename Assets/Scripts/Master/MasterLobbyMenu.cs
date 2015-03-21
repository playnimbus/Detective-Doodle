using UnityEngine;
using UnityEngine.UI;
using System;

public class MasterLobbyMenu : MonoBehaviour 
{
    public Action launchSessionClicked;
    public Action optionOneClicked;
    public Text roomName;

    public Text playersText;

    private GameNetwork network;

    void Start()
    {
        RefreshPlayersText();    
    }

    public void SetNetwork(GameNetwork network)
    {
        this.network = network;
        network.onPlayerConnected += RefreshPlayersText;
        network.onPlayerDisconnected += RefreshPlayersText;
        network.onPlayerPropertiesChanged += RefreshPlayersText;
    }

    void OnDestroy()
    {
        if (network != null)
        {
            network.onPlayerConnected -= RefreshPlayersText;
            network.onPlayerDisconnected -= RefreshPlayersText;
            network.onPlayerPropertiesChanged -= RefreshPlayersText;
        }
    }

#if UNITY_XBOXONE

    void Update()
    {
        if (XboxOneInput.GetKey(XboxOneKeyCode.GamepadButtonA))
        {
            LaunchSession();
        }
    }

#endif

    public void LaunchSession()
    {
        if (launchSessionClicked != null) launchSessionClicked();
    }

    public void OptionOne()
    {
        if (optionOneClicked != null) optionOneClicked();
    }

    public void SetRoomName(string name)
    {
        roomName.text = name;
    }

    public void RefreshPlayersText()
    {
        playersText.text = "";
        PhotonPlayer[] players = PhotonNetwork.otherPlayers;
        for(int i=0; i<players.Length; i++)
        {
            playersText.text = playersText.text + players[i].name + "\n";
        }
    }
}
