using UnityEngine;
using System;
using Object = System.Object;

public class CameraController : MonoBehaviour
{
    public GameObject _backgroundImage;
    private TankScript _vehicleScript;
    private float _offset, _targetOffset ,_vehicleWidth;
    
    void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerInitialized , OnPlayerInitialized);
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerMoveRight , OnPlayerMoveRight);
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerMoveLeft , OnPlayerMoveLeft);
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerStartMovingRight , OnPlayerStartMovingRight);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerInitialized , OnPlayerInitialized);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerMoveRight , OnPlayerMoveRight);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerMoveLeft , OnPlayerMoveLeft);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerStartMovingRight , OnPlayerStartMovingRight);
    }

    private void OnPlayerInitialized(Object data)
    {
        _vehicleScript = FindObjectOfType<TankScript>();
        _vehicleWidth = _vehicleScript.VehicleWidth;
    }
    
    private void OnPlayerStartMovingRight(Object data)
    {
        _offset = 0.0f;
        _targetOffset = _backgroundImage.transform.position.x - _vehicleScript.transform.position.x + _vehicleWidth * 0.5f;
    }

    private void OnPlayerMoveLeft(Object data)
    {
        _offset = 0.0f;
    }
    
    private void OnPlayerMoveRight(Object data)
    {
        float delta = float.Parse(data.ToString());
        _offset += delta;
        
        if (_offset > _targetOffset)
        {
            Vector3 position = gameObject.transform.position;
            position.x += delta;
            gameObject.transform.position = position;

            position = _backgroundImage.transform.position;
            position.x += delta;
            _backgroundImage.transform.position = position;
        }
    }
}
