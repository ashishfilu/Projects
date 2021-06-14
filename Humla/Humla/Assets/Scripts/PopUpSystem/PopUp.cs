using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public List<Button> _buttons;
    public List<Text> _buttonTexts;
    public Text _title;
    public Text _message;

    protected UnityAction _action1 = null;
    
    
    public void SetTitle(string text)
    {
        _title.text = text;
    }

    public void SetMessage(string text)
    {
        _message.text = text;
    }

    public void SetButtonText(string text , int index)
    {
        if (index >= 0 && index < _buttonTexts.Count)
        {
            _buttonTexts[index].text = text;   
        }
    }
    
    public void AddListenerToButton(UnityAction action , int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < _buttonTexts.Count)
        {
            _buttons[buttonIndex].onClick.AddListener(action);
        }
    }
    
    public void RemoveListenerFromButton(UnityAction action , int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < _buttonTexts.Count)
        {
            _buttons[buttonIndex].onClick.RemoveListener(action);
        }
    }
}
