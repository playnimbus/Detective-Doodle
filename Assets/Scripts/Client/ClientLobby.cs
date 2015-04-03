using UnityEngine;
using UnityEngine.UI;
using System;

public class ClientLobby : MonoBehaviour
{
    // UI fields
    public InputField field;
    public Button joinButton;
    public Button nameButton;
    public Text playersText;
    public InputField nameField;
    public InputField ipField;
    public Button ipButton;

    private GameNetwork network;

    void Start()
    {
        RefreshPlayersText();
        if (!string.IsNullOrEmpty(PhotonNetwork.player.name))
            nameField.text = PhotonNetwork.player.name;
    }

    #region Photon events

    void OnConnectedToMaster()
    {
        DisableIPUI();
        EnableRoomUI();
    }

    void OnJoinedRoom()
    {
        field.text = "Successfully joined room.";
        joinButton.interactable = false;
    }

    void OnPhotonJoinRoomFailed()
    {
        field.text = "Room not found.";
    }

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

    #endregion

    #region Button functions

    public void JoinRoom()
    {
        network.JoinRoom(field.text);
    }

    public void SetName()
    {
        PhotonNetwork.player.name = nameField.text;
        PlayerPrefs.SetString("name", nameField.text);
    }

    public void ConnectToIP()
    {
        network.ConnectWithIP(ipField.text);
    }

    #endregion

    public void SetNetwork(GameNetwork network)
    {
        this.network = network;
        if (network.settings.NeedIP)
        {
            ipField.text = network.settings.localSettings.localIP;
            DisableRoomUI();
        }
        else
        {
            DisableIPUI();
        }
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
            if (!players[i].isMasterClient)
                playersText.text = playersText.text + players[i].name + "\n";
        }
    }
}