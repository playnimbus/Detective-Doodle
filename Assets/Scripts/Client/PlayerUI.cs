using UnityEngine;
using UnityEngine.UI;
using System;

// Displays what a player has gathered.
public class PlayerUI : MonoBehaviour 
{
    [Serializable]
    public struct PlayerButton
    {
        public Button button;
        public Text text;
        public Action callback;
        public bool hideOnPressed;
    }

    public Image murdererIndicator;
    public Image detectiveIndicator;
    public Text headerText;
    public PlayerButton[] buttons;

    void Start()
    {
        murdererIndicator.enabled = false;
        detectiveIndicator.enabled = false;
        foreach (PlayerButton button in buttons)
            button.button.gameObject.SetActive(false);
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

    public void ShowButton(int num, string text, bool hideOnPressed, Action callback)
    {
        buttons[num].button.gameObject.SetActive(true);
        buttons[num].text.text = text;
        buttons[num].callback = callback;
        buttons[num].hideOnPressed = hideOnPressed;
    }
        
    public void HideAllButtons()
    {
        for (int num = 0; num < buttons.Length; num++)
        {
            buttons[num].button.gameObject.SetActive(false);
            buttons[num].callback = null;
        }
    }
    
    public void ButtonPressed(int num)
    {
        if (buttons[num].callback != null) buttons[num].callback();
        if (buttons[num].hideOnPressed) buttons[num].button.gameObject.SetActive(false);
    }

    public void SetHeaderText(string s)
    {
        headerText.text = s;
    }
}
