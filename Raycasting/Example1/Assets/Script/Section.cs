using UnityEngine;
using UnityEngine.Rendering;

public struct VoxelPoints
{
    public Vector3 Position;
    public float Weight;

    public static int GetSize()
    {
        return sizeof(float) * 4;
    }

    public VoxelPoints(Vector3 pos , float weight)
    {
        Position = pos;
        Weight = weight;
    }
};

public struct Triangle
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;

    public Vector3 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0:
                    return Vertex1;
                case 1:
                    return Vertex2;
                default:
                    return Vertex3;
            }
        }
    }
};


public class Section : MonoBehaviour
{
    public Vector3 Coordinate { get; set; }
    public Vector3 StartPosition { get; set; }
    public float Size { get; set; }
    public int VoxelPerAxis { get; set; }
    public int PointsPerAxis { get; set; }

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private Material _material;
    public Mesh SectionMesh { get; set; }
    
    public VoxelPoints[] AllPoints { get; set; }
    
    // Start is called before the first frame update
    void Start()
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
        }

        _meshCollider = GetComponent<MeshCollider>();
        if (_meshCollider == null)
        {
            _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        _meshRenderer.material = _material;
        SectionMesh = _meshFilter.sharedMesh;
        if (SectionMesh == null)
        {
            SectionMesh = new Mesh();
            SectionMesh.indexFormat = IndexFormat.UInt16;
            _meshFilter.sharedMesh = SectionMesh;
        }
    }

    public void SetParameters(Vector3 coordinate, float size , int pointsPerAxis)
    {
        Coordinate = coordinate;
        Size = size;
        
        StartPosition = new Vector3(Coordinate.x * size * 0.5f,
                                    Coordinate.y * size * 0.5f,
                                    Coordinate.z * size * 0.5f);
        
        PointsPerAxis = pointsPerAxis;
        VoxelPerAxis = pointsPerAxis-1;
        
        AllPoints = new VoxelPoints[pointsPerAxis*pointsPerAxis*pointsPerAxis];
        for (int i = 0; i < AllPoints.Length; i++)
        {
            AllPoints[i] = new VoxelPoints(Vector3.zero,0.0f);
        }
    }

    public void SetMaterial(Material material)
    {
        _material = material;
    }
    
    public void UpdateMesh(Triangle[] tris)
    {
        int numberOfTriangles = tris.Length;
        
        SectionMesh.Clear ();

        var vertices = new Vector3[numberOfTriangles * 3];
        var meshTriangles = new int[numberOfTriangles * 3];

        for (int i = 0; i < numberOfTriangles; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }

        SectionMesh.vertices = vertices;//MeshHelper.LaplacianFilter(vertices,meshTriangles);
        SectionMesh.triangles = meshTriangles;
        SectionMesh.RecalculateNormals ();
    }
}
