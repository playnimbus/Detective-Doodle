using UnityEngine;
using System;

public class SessionMenu : MonoBehaviour
{
    public Action buttonClicked;

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void Button()
    {
        if (buttonClicked != null) buttonClicked();
    }

    public void Toggle()
    {
        canvas.enabled = !canvas.enabled;
    }
}