#define DEBUG_CUBE

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarchingCube : MonoBehaviour
{
    [Range(3, 50)] public int _pointsPerAxis;
    [Range(1, 20)] public float _size; 
    [Range(0, 10)] public float _isoLevel;
    public Material _terrainMaterial;

    public Button _planarDensityButton, _sphereDensityButton, _noiseDensityButton;
    
    private DensityGenerator _densityGenerator;
    private DensityGenerator _planarDensityGenerator;
    private SphereDensity _sphereDensityGenerator;
    private NoiseDensity _noiseDensityGenerator;
    private ComputeShader _computeShader;

    private ComputeBuffer _triangleBuffer;
    private ComputeBuffer _pointsBuffer;
    private ComputeBuffer _triangleCountBuffer;
    
    private List<ComputeBuffer> _buffersToBeReleased;

    private GameObject _sectionRoot;
    private List<Section> _allSections;

    private int _kernelIndex;
    private static int _threadSize = 8;
    private int _sectionsPerAxis = 2;


#if DEBUG_CUBE
    private GameObject _debugBoxRoot;
    private GameObject _box;
    private List<GameObject> _allBoxes;
#endif

    // Start is called before the first frame update
    void Start()
    {
        _planarDensityGenerator = new DensityGenerator();
        _sphereDensityGenerator = new SphereDensity();
        _noiseDensityGenerator = new NoiseDensity();

        _densityGenerator = _planarDensityGenerator;
        _buffersToBeReleased = new List<ComputeBuffer>();
        
        _computeShader = Resources.Load("Shader/MarchingCube")as ComputeShader;
        _kernelIndex = _computeShader.FindKernel("CSMain");

        _allSections = new List<Section>();
        
#if DEBUG_CUBE        
        _allBoxes = new List<GameObject>();
        _debugBoxRoot = GameObject.Find("DebugCube");
        _box = Resources.Load("Prefabs/Cube")as GameObject;
#endif
        
        _planarDensityButton.onClick.AddListener(OnPlanarDensityButtonClicked);
        _sphereDensityButton.onClick.AddListener(OnSphericalDensityButtonClicked);
        _noiseDensityButton.onClick.AddListener(OnNoiseDensityButtonClicked);
        
        _sectionRoot = GameObject.Find("SectionRoot");
        InitializeSection();
        InitializeBuffers();
    }
    
    private void OnPlanarDensityButtonClicked()
    {
        _densityGenerator = _planarDensityGenerator;
    }
    
    private void OnNoiseDensityButtonClicked()
    {
        _densityGenerator = _noiseDensityGenerator;
    }

    private void OnSphericalDensityButtonClicked()
    {
        _densityGenerator = _sphereDensityGenerator;
    }
    
    private void InitializeSection()
    {
        for (int i = 0; i < _sectionsPerAxis; i++)
        {
            for (int j = 0; j < _sectionsPerAxis; j++)
            {
                GameObject sectionObject = new GameObject();
                Section section = sectionObject.AddComponent<Section>();
                section.SetMaterial(_terrainMaterial);
                section.SetParameters(new Vector3(i * 2 + 1, 1, j * 2 + 1), _size, _pointsPerAxis);
                sectionObject.name = "Section" + section.Coordinate.ToString();
                sectionObject.transform.parent = _sectionRoot.transform;
                sectionObject.transform.localPosition = new Vector3(0, 0, 0);

                _allSections.Add(section);

#if DEBUG_CUBE
                for (int k = 0; k < section.AllPoints.Length; k++)
                {
                    GameObject box = Instantiate(_box);
                    box.transform.parent = _debugBoxRoot.transform;
                    box.transform.localPosition = section.AllPoints[k].Position;
                    box.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                    _allBoxes.Add(box);
                }
#endif
            }
        }
    }
    
    private void InitializeBuffers()
    {
        int voxelsPerAxis = _pointsPerAxis - 1;
        int totalVoxelCount = voxelsPerAxis * voxelsPerAxis * voxelsPerAxis;
        int maxTriangleCount = totalVoxelCount * 5; 
        
        _pointsBuffer = new ComputeBuffer(_allSections[0].AllPoints.Length,VoxelPoints.GetSize());
        _triangleCountBuffer = new ComputeBuffer(1,sizeof(int),ComputeBufferType.Raw);
        _triangleBuffer = new ComputeBuffer(maxTriangleCount,sizeof(float)*3*3,ComputeBufferType.Append);
        
        _buffersToBeReleased.Add(_pointsBuffer);
        _buffersToBeReleased.Add(_triangleCountBuffer);
        _buffersToBeReleased.Add(_triangleBuffer);
    }
    
    void Update()
    {
        for (int i = 0; i < _allSections.Count; i++)
        {
            //Assign density and position to each voxel point
            _pointsBuffer.SetData(_allSections[i].AllPoints);
            _densityGenerator.Generate(_allSections[i],_pointsBuffer,_isoLevel);

#if DEBUG_CUBE        
            _pointsBuffer.GetData(_allSections[i].AllPoints);
            for (int j = 0; j < _allSections[i].AllPoints.Length; j++)
            {
                _allBoxes[j].transform.localPosition = _allSections[i].AllPoints[j].Position;
                float weight = _allSections[i].AllPoints[j].Weight;
                Material material = _allBoxes[i].GetComponent<Renderer>().material;
                material.color = new Vector4(weight, weight, weight, 1);
            }
#endif
        
            //Do cube marching
            _triangleBuffer.SetCounterValue(0);
            _computeShader.SetBuffer(_kernelIndex,"Points" , _pointsBuffer);
            _computeShader.SetBuffer(_kernelIndex , "Triangles" , _triangleBuffer);
            _computeShader.SetInt("PointsPerAxis",_pointsPerAxis);
            _computeShader.SetFloat("isoLevel" , _isoLevel);
        
            int threadPerAxis = Mathf.CeilToInt ((float)_pointsPerAxis / (float)_threadSize);
        
            _computeShader.Dispatch(_kernelIndex,threadPerAxis,threadPerAxis,threadPerAxis);
        
            ComputeBuffer.CopyCount(_triangleBuffer,_triangleCountBuffer,0);
            int[] countArray = {0};
            _triangleCountBuffer.GetData(countArray);
            int numberOfTriangles = countArray[0];
        
            Triangle[] tris = new Triangle[numberOfTriangles];
            _triangleBuffer.GetData(tris,0,0,numberOfTriangles);
        
            _allSections[i].UpdateMesh(tris);
            tris = null;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _buffersToBeReleased.Count; i++)
        {
            _buffersToBeReleased[i].Release();
        }
    }
}
