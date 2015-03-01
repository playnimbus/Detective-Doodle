using UnityEngine;
using System;

public class MasterLobbyMenu : MonoBehaviour 
{
    public Action launchSessionClicked;
    public Action optionOneClicked;

    public void LaunchSession()
    {
        if (launchSessionClicked != null) launchSessionClicked();
    }

    public void OptionOne()
    {
        if (optionOneClicked != null) optionOneClicked();
    }
}
