using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private bool _moveBot;
    private int _nodeIndex;
    private Vector3 _startPoint , _endPoint;
    private float _speed;
    private float _targetTime, _currentTime;

    private List<Vector3> Path;
    
    // Start is called before the first frame update
    void Start()
    {
        _moveBot = false;
        _nodeIndex = 0;
        _startPoint = _endPoint = Vector3.zero;
        _speed = 2.0f;
        _currentTime = _targetTime = 0.0f;
        Path = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveBot)
        {
            float timeRatio = _currentTime / _targetTime;
            Vector3 position = Vector3.Lerp(_startPoint, _endPoint, timeRatio);
            gameObject.transform.position = position;
            if (timeRatio >= 1.0f)
            {
                --_nodeIndex;
                SetNodeForMovement();
            }

            _currentTime += Time.deltaTime;
        }
    }

    public void CalculatePath(Vector3 gridStartPoint, Vector3 targetPoint,int numberOfGrids,Vector2 gridSize, 
                              List<Grid> grids,HeuristicType type = HeuristicType.Euclidean)

    {
        Vector3 currentPosition = gameObject.transform.position; 
        AStar.Instance.GetPath(gridStartPoint,currentPosition,targetPoint,numberOfGrids,
                        gridSize, grids,ref Path,type);

        if (Path.Count > 0)
        {
            _moveBot = true;
            _nodeIndex = Path.Count-1;
            SetNodeForMovement();
        }
    }

    private void SetNodeForMovement()
    {
        if (_nodeIndex > 0)
        {
            _startPoint = Path[_nodeIndex];
            _endPoint = Path[_nodeIndex-1];
            _currentTime = 0.0f;
            float distance = (_endPoint - _startPoint).magnitude;
            _targetTime = distance / _speed;
        }
        else
        {
            _moveBot = false;
        }
    }
    
    public void DrawPath()
    {
        if (Path != null)
        {
            for (int i = 1; i < Path.Count-1 ; i++)
            {
                Debug.DrawLine(Path[i],Path[i-1],Color.green);
            }
        }
    }
}
