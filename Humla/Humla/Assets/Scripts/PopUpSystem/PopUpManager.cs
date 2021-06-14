using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : Singleton<PopUpManager>
{
    private Stack<GameObject> _stack;
    
    public PopUpManager()
    {
        _stack = new Stack<GameObject>();
    }

    public GameObject PushToStack(string path)
    {
        GameObject popup = GameObject.Instantiate(Resources.Load(path)as GameObject);
        if (_stack.Count > 0)
        {
            _stack.Peek().SetActive(false);
        }
        _stack.Push(popup);

        Canvas root = GameObject.FindObjectOfType<Canvas>();
        popup.transform.parent = root.transform;
        popup.transform.localPosition = Vector3.zero;
        return popup;
    }

    public void PopFromStack()
    {
        GameObject tos = _stack.Pop();
        GameObject.Destroy(tos);
    }

    public void Clear()
    {
        while (_stack.Count > 0 )
        {
            PopFromStack();
        }
    }
}
