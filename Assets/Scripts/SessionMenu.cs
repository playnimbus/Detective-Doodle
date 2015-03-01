using UnityEngine;
using System;

public class SessionMenu : MonoBehaviour
{
    public Action buttonClicked;

    public void Button()
    {
        if (buttonClicked != null) buttonClicked();
    }
}