using UnityEngine;

public struct Ray 
{
    float _length ;
    Vector3 _hitPoint;
    GameObject _line;
    Vector3 _localPosition;
    Vector3 _direction;

    public Vector3 Direction => _direction;

    public Ray(GameObject line, GameObject root, Vector3 localPosition, Vector3 direction)
    {
        _length = 0;
        _hitPoint = Vector3.negativeInfinity;
        _line = GameObject.Instantiate(line);
        _localPosition = localPosition;
        _direction = direction;

        _line.transform.parent = root.transform;
        RectTransform rectTransform = _line.GetComponent<RectTransform>();
        rectTransform.localPosition = _localPosition;
        float angle = Mathf.Acos(Vector3.Dot(Vector3.right, _direction));
        float sign = Vector3.Cross(Vector3.right, _direction).z >= 0 ? 1.0f : -1.0f;
        rectTransform.localRotation = Quaternion.Euler(0, 0, sign * Mathf.Rad2Deg * angle);
    }

    public Ray(GameObject line, GameObject root, Vector3 localPosition, float angleInDegree)
    {
        _length = 0;
        _hitPoint = Vector3.negativeInfinity;
        _line = GameObject.Instantiate(line);
        _localPosition = localPosition;
        _direction = Quaternion.Euler(0, 0, angleInDegree) * Vector3.right;

        _line.transform.parent = root.transform;
        RectTransform rectTransform = _line.GetComponent<RectTransform>();
        rectTransform.localPosition = _localPosition;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angleInDegree);
    }

    public void SetHitPoint(Vector3 hitPoint)
    {
        _hitPoint = hitPoint;
        _length = (_hitPoint - GetWorldPosition()).magnitude;
        UpdateRayLength();
    }

    public void UpdateRayLength()
    {
        if (_length != Mathf.Infinity)
        {
            RectTransform rectTransform = _line.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(_length, 1, 1);
        }
    }

    public Vector3 GetWorldPosition()
    {
        RectTransform rectTransform = _line.GetComponent<RectTransform>();
        return rectTransform.position;
    }
}
