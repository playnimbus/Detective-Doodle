using UnityEngine;
using UnityEngine.UI;
using System;

// Displays what a player has gathered.
public class PlayerUI : MonoBehaviour 
{
    public Image murdererIndicator;
    public Text evidenceText;
    public Button button;
    public Text buttonText;

    private Action buttonCallback;

    void Start()
    {
        murdererIndicator.enabled = false;
        button.gameObject.SetActive(false);
    }

    public void MarkAsMurderer()
    {
        // HACK
        Invoke("Foo", 0.5f);
    }
    
    void Foo() { murdererIndicator.enabled = true; }

    public void ShowButton(string text, Action callback)
    {
        button.gameObject.SetActive(true);
        buttonText.text = text;
        buttonCallback = callback;
    }

    public void HideButton()
    {
        button.gameObject.SetActive(false);
        buttonCallback = null;
    }
    
    public void ButtonPressed()
    {
        if (buttonCallback != null) buttonCallback();
        HideButton();
    }

    public void SetEvidenceText(string s)
    {
        evidenceText.text = s;
    }
}
