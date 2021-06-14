using UnityEngine;
using UnityEngine.EventSystems;

public enum JoystickDirection
{
    Positive,
    Negative,
    Zero
}

public delegate void OnMoveAlongXAxis(JoystickDirection direction,float delta);
public delegate void OnMoveAlongYAxis(JoystickDirection direction,float delta);

public delegate void OnReset();

public class JoystickController : MonoBehaviour
{
    public CustomButton _button;
    private float _radius;
    private bool _buttonPressed;
    private Vector2 _previousTouchPosition;

    public event OnMoveAlongXAxis onMoveAlongXAxis;
    public event OnMoveAlongYAxis onMoveAlongYAxis;
    public event OnReset onReset;
    
    // Start is called before the first frame update
    void Start()
    {
        _radius = gameObject.GetComponent<RectTransform>().rect.width * 0.5f;
        _radius /= Mathf.Pow(2, 0.5f);
        _buttonPressed = false;
        _button.onButtonPressed += OnJoystickButtonPressed;
        _button.onButtonReleased += OnJoystickButtonReleased;
    }

    // Update is called once per frame
    void Update()
    {
        if (_buttonPressed)
        {
#if UNITY_EDITOR
            Vector3 mousePosition = Input.mousePosition;
            Vector2 delta = new Vector2( mousePosition.x -_previousTouchPosition.x  , mousePosition.y - _previousTouchPosition.y);
            _previousTouchPosition = mousePosition;
            
            {
                Vector3 buttonLocalPosition = _button.gameObject.transform.localPosition;
                buttonLocalPosition.x += delta.x;
                buttonLocalPosition.y += delta.y;
                
                if (buttonLocalPosition.x < _radius && buttonLocalPosition.x > -_radius
                    && buttonLocalPosition.y < _radius && buttonLocalPosition.y > -_radius )
                {
                    if (buttonLocalPosition.x > 0.0f)
                    {
                        onMoveAlongXAxis?.Invoke( JoystickDirection.Positive, delta.x);
                    }
                    else if (buttonLocalPosition.x < 0.0f)
                    {
                        onMoveAlongXAxis?.Invoke(JoystickDirection.Negative,delta.x);
                    }
                    if (buttonLocalPosition.y > 0.0f)
                    {
                        onMoveAlongYAxis?.Invoke(JoystickDirection.Positive, delta.y);
                    }
                    else if (buttonLocalPosition.y < 0.0f)
                    {
                        onMoveAlongYAxis?.Invoke( JoystickDirection.Negative, delta.y);
                    }
                    _button.gameObject.transform.localPosition = buttonLocalPosition;
                }
            }
            
#else
            if (Input.touchCount <= 0) return;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector2 position = Input.GetTouch(i).position;
                
                if (position.x < Screen.width * 0.5f)
                {
                    Vector2 delta = Input.GetTouch(i).deltaPosition;
                    Vector3 buttonLocalPosition = _button.gameObject.transform.localPosition;
                    buttonLocalPosition.x += delta.x * 0.25f;
                    buttonLocalPosition.y += delta.y * 0.25f;
    
                    if (buttonLocalPosition.x > 0.0f)
                    {
                        onMoveAlongXAxis?.Invoke(JoystickDirection.Positive,delta.x);
                    }
                    else if (buttonLocalPosition.x < 0.0f)
                    {
                        onMoveAlongXAxis?.Invoke(JoystickDirection.Negative,delta.x);
                    }
                    if (buttonLocalPosition.y > 0.0f)
                    {
                        onMoveAlongYAxis?.Invoke(JoystickDirection.Positive,delta.y);
                    }
                    else if (buttonLocalPosition.y < 0.0f)
                    {
                        onMoveAlongYAxis?.Invoke(JoystickDirection.Negative,delta.y);
                    }
                    if (buttonLocalPosition.x < _radius && buttonLocalPosition.x > -_radius
                        && buttonLocalPosition.y < _radius && buttonLocalPosition.y > -_radius )
                    {
                        _button.gameObject.transform.localPosition = buttonLocalPosition;
                    }
                    break;
                }
            }
#endif
        }
    }

    private void OnJoystickButtonPressed(PointerEventData data)
    {
        _buttonPressed = true;
        _previousTouchPosition = data.position;
    }

    private void OnJoystickButtonReleased(PointerEventData data)
    {
        _buttonPressed = false;
        _button.gameObject.transform.localPosition = Vector3.zero;
        onReset?.Invoke();
    }
}
