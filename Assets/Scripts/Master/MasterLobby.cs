using UnityEngine;
using UnityEngine.UI;
using System;

public class MasterLobby : MonoBehaviour 
{
    // UI fields
    public Text roomName;
    public Text playersText;

    // This gets activated when the session is to start
    public Action<byte> onLaunchSession;

    private GameNetwork network;

    void Start()
    {
        RefreshPlayersText();
    }

    // Begin Photon events

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        RefreshPlayersText();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        RefreshPlayersText();
    }

    void OnPhotonPlayerPropertiesChanged(object[] playerAndProperties)
    {
        RefreshPlayersText();
    }

    // End Photon events

#if UNITY_XBOXONE

    void Update()
    {
        if (XboxOneInput.GetKey(XboxOneKeyCode.GamepadButtonA))
        {
            LaunchSession();
        }
    }

#endif

    // Launch Button function

    public void LaunchSession()
    {
        if (onLaunchSession != null) onLaunchSession(SessionType.Whodunnit);
    }

    public void SetNetwork(GameNetwork network)
    {
        this.network = network;
    }

    public void SetRoomName(string name)
    {
        roomName.text = name;
    }

    public void RefreshPlayersText()
    {
        playersText.text = "";
        PhotonPlayer[] players = PhotonNetwork.otherPlayers;
        for (int i = 0; i < players.Length; i++)
        {
            playersText.text = playersText.text + players[i].name + "\n";
        }
    }

}
