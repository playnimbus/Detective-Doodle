using UnityEngine;
using UnityEngine.UI;
using System;

// Displays what a player has gathered.
public class PlayerUI : MonoBehaviour 
{
    public Image murdererIndicator;
    public Image detectiveIndicator;
    public Text headerText;
    public Button button;
    public Text buttonText;

    private Action buttonCallback;
    private bool hideButtonOnPressed;

    void Start()
    {
        murdererIndicator.enabled = false;
        detectiveIndicator.enabled = false;
        button.gameObject.SetActive(false);
    }

    public void MarkAsMurderer()
    {
        // HACK
        Invoke("M", 0.5f);
    }
    
    void M() { murdererIndicator.enabled = true; }

    public void MarkAsDetective(bool val)
    {
        // HACK
        if(val) Invoke("D", 0.5f);
        else detectiveIndicator.enabled = false;
    }

    void D() { detectiveIndicator.enabled = true; }

    public void ShowButton(string text, Action callback, bool hideOnPressed)
    {
        button.gameObject.SetActive(true);
        buttonText.text = text;
        buttonCallback = callback;
        hideButtonOnPressed = hideOnPressed;
    }

    public void HideButton()
    {
        button.gameObject.SetActive(false);
        buttonCallback = null;
    }
    
    public void ButtonPressed()
    {
        if (buttonCallback != null) buttonCallback();
        if (hideButtonOnPressed) HideButton();
    }

    public void SetHeaderText(string s)
    {
        headerText.text = s;
    }
}
