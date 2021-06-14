using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _mainCamera;
    private GameObject _targetObject;
    private Vector2 _previousMousePoint;
    private float _distance;
    private Vector3 _lookAt;
    private bool _isMouseDown = false;
    
    private bool _freeSpinActive;
    private float _time , _freeSpinSpeed , _freeSpinDirection;
    private Vector2 _mousePressedPosition,_mouseReleasedPosition;
    private float _previousTouchDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _targetObject = GameObject.Find("Ground");
        
        _previousMousePoint = Vector2.positiveInfinity;

        Vector3 carPosition = _targetObject.transform.position;
        _distance = (carPosition - _mainCamera.transform.position).magnitude;
        _lookAt = _mainCamera.transform.forward;
        
        _mainCamera.transform.forward = _lookAt;
        _mainCamera.transform.position = _targetObject.transform.position - _lookAt * _distance;

        _time = 0.0f;
        _freeSpinSpeed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    { 
        _previousTouchDistance = Mathf.Infinity;
        if (Input.GetMouseButtonDown(0))
        {
            RegisterMouseDown();
            _time = 0.0f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            RegisterMouseRelease();
        }
        
        if (Input.mouseScrollDelta.y > 0)
        {
            //Zoom in 
            float fov = _mainCamera.fieldOfView;
            fov -= 1.0f;
            if (fov <= 5)
            {
                fov = 5;
            }
            _mainCamera.fieldOfView = fov;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            //Zoom out 
            float fov = _mainCamera.fieldOfView;
            fov += 1.0f;
            if (fov >= 60)
            {
                fov = 60;
            }
            _mainCamera.fieldOfView = fov;
        }
        
        if( _isMouseDown )
        {
            UpdateCamera();
        }

        _time += Time.deltaTime;

        if (_freeSpinActive)
        {
            UpdateCameraFreeSpin();
        }
    }

    private void RegisterMouseDown()
    {
        _freeSpinActive = false;
        _isMouseDown = true;
        _previousMousePoint = Vector2.positiveInfinity;
#if UNITY_EDITOR
        _mousePressedPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
#else                
        _mousePressedPosition = Input.GetTouch(0).position;
#endif

    }

    private void RegisterMouseRelease()
    {
        _isMouseDown = false;
        _freeSpinActive = false;
        _previousMousePoint = Vector2.positiveInfinity;
#if UNITY_EDITOR
        _mouseReleasedPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
#else                
        _mouseReleasedPosition = Input.GetTouch(0).position;
#endif

        float distanceX = _mouseReleasedPosition.x - _mousePressedPosition.x;
        float distanceY = _mouseReleasedPosition.y - _mousePressedPosition.y;

        if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
        {
            float absDistance = Mathf.Abs(distanceX);
            if (absDistance > 0)
            {
                _freeSpinDirection = absDistance / distanceX;
                _freeSpinSpeed = absDistance /(_time * 50.0f);
                _time = 0.0f;
                _freeSpinActive = true;
            }
        }
    }

    private void UpdateCamera()
    {
        Vector2 currentMousePosition = Vector3.zero;
#if UNITY_EDITOR
        currentMousePosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
#else                
            currentMousePosition = Input.GetTouch(0).position;
#endif
        if (_previousMousePoint.x != Vector2.positiveInfinity.x
            && _previousMousePoint != currentMousePosition )
        {
            Vector3 worldPoint1 = _mainCamera.ScreenToWorldPoint(new Vector3(_previousMousePoint.x, _previousMousePoint.y,
                _mainCamera.farClipPlane));
            Vector3 worldPoint2 = _mainCamera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y,
                _mainCamera.farClipPlane));
            
            float xDistance = Mathf.Abs(_previousMousePoint.x - currentMousePosition.x);
            float yDistance = Mathf.Abs(_previousMousePoint.y - currentMousePosition.y);
            
            worldPoint1.Normalize();
            worldPoint2.Normalize();

            float angle = Mathf.Acos(Vector3.Dot(worldPoint2,worldPoint1)) * Mathf.Rad2Deg;
            if (xDistance > yDistance)
            {
                float sign = _previousMousePoint.x > currentMousePosition.x ? -1.0f:1.0f;
                
                _lookAt = Quaternion.AngleAxis(sign*angle,Vector3.up) *_lookAt;
                _mainCamera.transform.forward = _lookAt;
                _mainCamera.transform.position = _targetObject.transform.position - _lookAt * _distance;
            }
            else
            {
                float sign = _previousMousePoint.y > currentMousePosition.y ? 1.0f:-1.0f;
                _lookAt = Quaternion.AngleAxis(sign*angle,_mainCamera.transform.right) *_lookAt;
                
                Vector3 tempPosition = _targetObject.transform.position - _lookAt * _distance;
                if (tempPosition.y > 0.4f && tempPosition.y < 3.0f)
                {
                    _mainCamera.transform.forward = _lookAt;   
                    _mainCamera.transform.position = _targetObject.transform.position - _lookAt * _distance;
                }   
            }
        }
        _previousMousePoint = currentMousePosition;
    }

    private void UpdateCameraFreeSpin()
    {
        float currentAngle = Time.deltaTime * _freeSpinDirection * _freeSpinSpeed ;
        if (currentAngle != Double.NaN)
        {
            _lookAt = _lookAt = Quaternion.AngleAxis(currentAngle,Vector3.up) *_lookAt;
            _mainCamera.transform.forward = _lookAt;
            _mainCamera.transform.position = _targetObject.transform.position - _lookAt * _distance;
            _freeSpinSpeed -= 1.0f;
            if (_freeSpinSpeed <= 0.0f)
            {
                _freeSpinSpeed = 0.0f;
                _freeSpinActive = false;
            }
        }
        else
        {
            _freeSpinSpeed = 0.0f;
            _freeSpinActive = false;
        }
    }
}
