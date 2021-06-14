#define DRAW_GRID

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class Grid
{
    public Vector3 Position;
    public Vector3 TopLeft, TopRight, BottomRight, BottomLeft;
    public bool Active;
    public int IndexX, IndexZ;

    public float F, G, H;
    public Grid Parent;

    public Grid(Vector3 topLeft , Vector3 topRight , Vector3 bottomRight , Vector3 bottomLeft,
                int indexX , int indexZ )
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;

        IndexX = indexX;
        IndexZ = indexZ;
        
        Active = true;

        Position.x = Mathf.Lerp(topLeft.x, topRight.x, 0.5f);
        Position.y = Mathf.Lerp(topLeft.y, bottomLeft.y, 0.5f);
        Position.z = Mathf.Lerp(topLeft.z, bottomLeft.z, 0.5f);

        F = G = H = float.MaxValue;
        Parent = null;
    }
    
    public void Draw()
    {
        if (!Active)
        {
            return;
        }
        Debug.DrawLine(TopLeft,TopRight,Color.red);
        Debug.DrawLine(TopRight,BottomRight,Color.red);
        Debug.DrawLine(BottomLeft,BottomRight,Color.red);
        Debug.DrawLine(TopLeft,BottomLeft,Color.red);
    }

    public void Reset()
    {
        F = G = H = float.MaxValue;
        Parent = null;
    }
}

public class NavMesh : MonoBehaviour
{
    public Vector2 _totalSize;
    public int _numberOfGrids;
    public Material _material;
    public GameObject _ground;
    public Bot _bot;

    public Button _euclideanButton, _manhattanButton, _diagonalButton;
    
    private List<Grid> _grids;
    private Vector3 _startPosition;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _sharedMesh;
    private Vector2 _gridSize;
    
    private HeuristicType _heuristicType;
    
    private void Start()
    {
        _manhattanButton.onClick.AddListener(OnManhattanClicked);
        _euclideanButton.onClick.AddListener(OnEuclideanClicked);
        _diagonalButton.onClick.AddListener(OnDiagonalClicked);
        
        _grids = new List<Grid>();
        _startPosition = gameObject.transform.position;
        _gridSize = _totalSize / _numberOfGrids;
        
        var vertices = new List<Vector3>();

        Vector3 startPoint = _startPosition;
        for (int i = 0; i < _numberOfGrids+1; i++)
        {
            for (int j = 0; j < _numberOfGrids+1; j++)
            {
                vertices.Add(startPoint);
                startPoint.x -= _gridSize.x;
            }
            startPoint.x = _startPosition.x;
            startPoint.z += _gridSize.y;
        }

        for (int i = 0; i < _numberOfGrids; i++)
        {
            for (int j = 0; j < _numberOfGrids; j++)
            {
                int index = i * (_numberOfGrids + 1) + j;
                Vector3 topLeft = vertices[index];
                Vector3 topRight = vertices[index+1];
                Vector3 bottomRight = vertices[index+_numberOfGrids+2];
                Vector3 bottomLeft = vertices[index+_numberOfGrids+1];
                
                Grid temp = new Grid(topLeft,topRight,bottomRight,bottomLeft,j,i);
                _grids.Add(temp);
            }
        }

        _ground.SetActive(false);
        for (int i = 0; i < _grids.Count; i++)
        {
            Collider[] collders = Physics.OverlapBox(_grids[i].Position,
                new Vector3(_gridSize.x * 0.5f, 0.5f, _gridSize.y * 0.5f));

            if (collders.Length > 0)
            {
                _grids[i].Active = false;
            }
        }
        _ground.SetActive(true);
#if DRAW_GRID
        //AddMeshComponents();
        //CreateMesh(vertices);
#endif
        gameObject.transform.position = Vector3.zero;
        
        _heuristicType = HeuristicType.Euclidean;
    }

    private void AddMeshComponents()
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = _material;
        }

        _sharedMesh = _meshFilter.sharedMesh;
        if (_sharedMesh == null)
        {
            _sharedMesh = new Mesh();
            _sharedMesh.indexFormat = IndexFormat.UInt16;
            _meshFilter.sharedMesh = _sharedMesh;
        }
    }

    private void CreateMesh(List<Vector3> vertices)
    {
        var indices = new List<int>();

        int numberOfColumns = Mathf.CeilToInt(Mathf.Pow(_grids.Count, 0.5f)); 
        
        for (int i = 0; i < _grids.Count; i++)
        {
            int index = _grids[i].IndexX * (_numberOfGrids + 1) + _grids[i].IndexZ;
            
            indices.Add( index+_numberOfGrids+1  );
            indices.Add( index );
            indices.Add( index + 1 );
            
            indices.Add( index+_numberOfGrids+1 );
            indices.Add( index + 1 );
            indices.Add( index+_numberOfGrids+2 );
        }

        _sharedMesh.vertices = vertices.ToArray();
        _sharedMesh.triangles = indices.ToArray();
        _sharedMesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
#if DRAW_GRID
        if (_grids != null)
        {
            for (int i = 0; i < _grids.Count; i++)
            {
                _grids[i].Draw();
            }
        }
        _bot.DrawPath();
#endif
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                ResetAllGrids();
                _bot.CalculatePath(_startPosition, hitInfo.point,_numberOfGrids,_gridSize,_grids,_heuristicType);
            }
        }
    }

    private void OnEuclideanClicked()
    {
        _heuristicType = HeuristicType.Euclidean;
    }
    private void OnManhattanClicked()
    {
        _heuristicType = HeuristicType.Manhattan;
    }
    private void OnDiagonalClicked()
    {
        _heuristicType = HeuristicType.Diagonal;
    }

    private void ResetAllGrids()
    {
        for (int i = 0; i < _grids.Count; i++)
        {
            _grids[i].Reset();
        }
    }
}
