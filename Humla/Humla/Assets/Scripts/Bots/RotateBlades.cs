using System;
using UnityEngine;
using Object = System.Object;

public class RotateBlades : MonoBehaviour
{
    public float _speed;
    public Vector3 _axis;

    private bool _gamePaused;
    
    void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    void Update()
    {
        if (_gamePaused)
        {
            return;
        }
        gameObject.transform.rotation *= Quaternion.Euler(_axis*_speed); 
    }
    
    private void OnPausePressed(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
    }
}
