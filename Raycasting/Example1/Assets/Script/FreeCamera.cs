using System;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector2 _previousMousePoint;
    private bool _isMouseDown = false;
    private Vector3 _lookAt;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _previousMousePoint = Vector2.positiveInfinity;
        _lookAt = _mainCamera.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isMouseDown = false;
            _previousMousePoint = Vector2.positiveInfinity;
        }

        if (Input.GetKey(KeyCode.W))
        {
            Vector3 position = _mainCamera.transform.position;
            position += _mainCamera.transform.forward * 10 * Time.deltaTime;
            _mainCamera.transform.position = position;
        } 
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 position = _mainCamera.transform.position;
            position -= _mainCamera.transform.forward * 10 * Time.deltaTime;
            _mainCamera.transform.position = position;
        } 
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 position = _mainCamera.transform.position;
            position -= _mainCamera.transform.right * 10 * Time.deltaTime;
            _mainCamera.transform.position = position;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 position = _mainCamera.transform.position;
            position += _mainCamera.transform.right * 10 * Time.deltaTime;
            _mainCamera.transform.position = position;
        }
        
        if (_isMouseDown)
        {
            Vector2 currentMousePosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
            
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
                    Vector3 temp = Quaternion.AngleAxis(sign*angle,Vector3.up) *_lookAt;
                    if (temp.x != Double.NaN && temp.y != Double.NaN && temp.z != Double.NaN)
                    {
                        _lookAt = temp;
                        _mainCamera.transform.forward = _lookAt;
                    }
                }
                else
                {
                    float sign = _previousMousePoint.y > currentMousePosition.y ? 1.0f:-1.0f;
                    Vector3 temp = Quaternion.AngleAxis(sign*angle,_mainCamera.transform.right) *_lookAt;
                    if (temp.x != Double.NaN && temp.y != Double.NaN && temp.z != Double.NaN)
                    {
                        _lookAt = temp;
                        _mainCamera.transform.forward = _lookAt;
                    }
                }
            }
            _previousMousePoint = currentMousePosition;
        }
    }
}
