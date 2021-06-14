using UnityEngine;

public class TerrainLayoutController : MonoBehaviour
{
    [SerializeField]
    private GameObject _startPoint;
    [SerializeField]
    private GameObject _endPoint;
    
    
    public Vector3 GetStartPoint()
    {
        return _startPoint.transform.position;
    }
    
    public Vector3 GetEndPoint()
    {
        return _endPoint.transform.position;
    }
}


