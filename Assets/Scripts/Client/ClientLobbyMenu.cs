using UnityEngine;
using UnityEngine.UI;
using System;

public class ClientLobbyMenu : MonoBehaviour
{
    public InputField field;
    public Button joinButton;
    public Button nameButton;
    public Text playersText;
    public InputField nameField;
    public InputField ipField;
    public Button ipButton;

    public Action<string> joinRoomRequested;
    public Action<string> nameChangedRequested;

    private GameNetwork network;
    
    void Start()
    {
        RefreshPlayersText();
        if (!string.IsNullOrEmpty(PhotonNetwork.player.name))
            nameField.text = PhotonNetwork.player.name;
    }

    public void JoinRoom()
    {
        if (joinRoomRequested != null) joinRoomRequested(field.text);
    }

    public void JoinRoomFailed()
    {
        field.text = "Room not found.";
    }

    public void JoinRoomSucceeded()
    {
        field.text = "Successfully joined room.";
        joinButton.interactable = false;
    }

    public void SetNetwork(GameNetwork network)
    {
        this.network = network;
        network.onPlayerConnected += RefreshPlayersText;
        network.onPlayerDisconnected += RefreshPlayersText;
        network.onPlayerPropertiesChanged += RefreshPlayersText;
        network.onConnected += DisableIPUI;
        network.onConnected += EnableRoomUI;

        if (network.useLocalServer && !network.localSettings.connectAutomatically)
        {
            ipField.text = network.localSettings.localIP;
            DisableRoomUI();
        }
        else
        {
            DisableIPUI();
        }
    }

    void OnDestroy()
    {
        if (network != null)
        {
            network.onPlayerConnected -= RefreshPlayersText;
            network.onPlayerDisconnected -= RefreshPlayersText;
            network.onPlayerPropertiesChanged -= RefreshPlayersText;
            network.onConnected -= DisableIPUI;
            network.onConnected -= EnableRoomUI;
        }
    }
    
    public void SetName()
    {
        if (nameChangedRequested != null) nameChangedRequested(nameField.text);
    }

    public void ConnectToIP()
    {
        network.ConnectWithIP(ipField.text);
    }

    void DisableIPUI()
    {
        ipField.interactable = false;
        ipButton.interactable = false;
    }

    void DisableRoomUI()
    {
        field.interactable = false;
        joinButton.interactable = false;
    }

    void EnableRoomUI()
    {
        field.interactable = true;
        joinButton.interactable = true;
    }

    public void RefreshPlayersText()
    {
        playersText.text = "";
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; i++)
        {
            if(!players[i].isMasterClient)
                playersText.text = playersText.text + players[i].name + "\n";
        }
    }
}