using UnityEngine;
using UnityEngine.UI;
using System;

public class SessionMenu : MonoBehaviour
{
    public Action buttonClicked;
    public Text header;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void SetHeader(string s, bool show)
    {
        header.text = s;
        if (show) canvas.enabled = true;
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