using System.Collections;
using UnityEngine;
using Object = System.Object;

public class AimController : MonoBehaviour
{
    private Vector3 _lastTouchPosition;
    private bool _processTouch = false;
    private bool _gamePaused = false;

    private void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    private void Update()
    {
        if (_gamePaused)
        {
            return;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.x > Screen.width * 0.2f)
            {
                if (!_processTouch)
                {
                    _processTouch = true;
                    _lastTouchPosition = Input.mousePosition;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _processTouch = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            
            if (mousePosition.x > 0.2f && _processTouch)
            {
                Vector3 position = gameObject.transform.position;
                position += Input.mousePosition - _lastTouchPosition;
                
                if (position.x > 0 && position.x < Screen.width &&
                    position.y > 0 && position.y < Screen.height)
                {
                    _lastTouchPosition = Input.mousePosition;
                    gameObject.transform.position = position;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                    worldPosition.z = 0.0f;
                    GameEventManager.Instance.TriggerEvent(Event.RightTouchUpdated , worldPosition.ToString());
                }
            }
        }
#else
        if (Input.touchCount <= 0) return;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector2 touchPosition = Input.GetTouch(i).position;

            if (touchPosition.x > Screen.width * 0.2f)
            {
                Vector3 position = gameObject.transform.position;
                Vector2 delta = Input.GetTouch(i).deltaPosition;
                position.x += delta.x;
                position.y += delta.y;
                
                if (position.x > 0 && position.x < Screen.width &&
                    position.y > 0 && position.y < Screen.height)
                {
                    gameObject.transform.position = position;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                    worldPosition.z = 0.0f;
                    GameEventManager.Instance.TriggerEvent(Event.RightTouchUpdated , worldPosition.ToString());
                }
            }
        }
#endif
    }

    private void OnPausePressed(Object data)
    {
        _gamePaused = bool.Parse(data.ToString());
    }
}
