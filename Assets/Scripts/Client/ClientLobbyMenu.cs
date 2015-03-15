using UnityEngine;
using UnityEngine.UI;
using System;

public class ClientLobbyMenu : MonoBehaviour
{
    public Action<string> joinRoomRequested;
    public InputField field;
    public Button joinButton;

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
        field.interactable = false;
        joinButton.interactable = false;
    }
}