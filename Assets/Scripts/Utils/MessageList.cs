using UnityEngine;
using System.Collections.Generic;

public class MessageList : MonoBehaviour
{
    static readonly int max = 5;

    string[] messages = new string[max];
    int count = 0;
    bool visible = false;

    public void AddMessage(string message)
    {
        if (count == max)
        {
            for (int i = 0; i < max-1; i++)
                messages[i] = messages[i + 1];
            count--;
        }

        messages[count] = message;
        count++;
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(5, Screen.height - 105, 100, 100), "Messages"))
            visible = !visible;

        if (visible)
        {
            int y = 5;
            for (int i = 0; i < count; i++)
            {
                GUI.Label(new Rect(5, y, 250, 25), messages[i]);
                y += 18;
            }
        }
    }
}
