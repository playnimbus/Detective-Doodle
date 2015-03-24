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
    
    public void SetName()
    {
        if (nameChangedRequested != null) nameChangedRequested(nameField.text);
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