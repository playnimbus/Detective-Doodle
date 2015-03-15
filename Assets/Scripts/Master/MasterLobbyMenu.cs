using UnityEngine;
using UnityEngine.UI;
using System;

public class MasterLobbyMenu : MonoBehaviour 
{
    public Action launchSessionClicked;
    public Action optionOneClicked;
    public Text roomName;

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
}
