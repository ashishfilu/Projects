using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public struct ShapeData
{
    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 Color;
    public int Type;
    public int OperationType;
    public static int GetSize()
    {
        return sizeof(float) * 9 + sizeof(int) * 2;
    }
}

[ExecuteInEditMode,ImageEffectAllowedInSceneView]
public class Raymarch : MonoBehaviour
{
    public ComputeShader _computeShader;
    public Button _softShadowButton;
    public Text _buttonText;

    private int _kernelIndex;
    private RenderTexture _renderTarget;
    private Light _light;
    private Camera _camera;
    private List<ComputeBuffer> _computeBuffersToDispose;
    private float _enableSoftShadow=0;
    private Texture _earthTexture; 

    private void Initialize()
    {
        _computeBuffersToDispose = new List<ComputeBuffer>();
        _light = FindObjectOfType<Light>();
        _camera = Camera.main;
        _kernelIndex = _computeShader.FindKernel("CSMain");
        _softShadowButton.onClick.AddListener(OnButtonClicked);
        _earthTexture = Resources.Load<Texture>("Textures/2k_earth_daymap")as Texture;
    }

    private void OnButtonClicked()
    {
        _enableSoftShadow = _enableSoftShadow == 1.0f ? 0 : 1;
        if (_enableSoftShadow == 1.0f)
        {
            _buttonText.text = "SoftShadow";
        }
        else
        {
            _buttonText.text = "HardShadow";
        }
    }
    
    private void InitRenderTexture()
    {
        if (_renderTarget != null)
        {
            _renderTarget.Release();
        }

        _renderTarget = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear) {enableRandomWrite = true};
        _renderTarget.Create();
    }

    private void Start()
    {
        Initialize();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        InitRenderTexture();
        _computeBuffersToDispose.Clear();
        CreateSceneData();
        SetParameters();
        
        //Set source and destination texture in Compute shader
        _computeShader.SetTexture(_kernelIndex,"Source",src);
        _computeShader.SetTexture(_kernelIndex,"Destination",_renderTarget);
        _computeShader.SetTexture(_kernelIndex,"EarthTexture", _earthTexture);
        
        //Calculate threads
        int threadXGroup = Mathf.CeilToInt(_camera.pixelWidth / 8.0f);
        int threadYGroup = Mathf.CeilToInt(_camera.pixelHeight / 8.0f);
        _computeShader.Dispatch(_kernelIndex,threadXGroup,threadYGroup,1);

        Graphics.Blit(_renderTarget,dest);
        
        for (int i = 0; i < _computeBuffersToDispose.Count; i++)
        {
            _computeBuffersToDispose[i].Dispose();
        }
    }

    private void CreateSceneData()
    {
        List<Shape> allShapes = new List<Shape>(FindObjectsOfType<Shape>());
        ShapeData[] allShapeData = new ShapeData[allShapes.Count];

        for (int i = 0; i < allShapes.Count; i++)
        {
            allShapeData[i] = new ShapeData()
            {
                Position = allShapes[i].Position,
                Scale = allShapes[i].Scale,
                Color = new Vector3(allShapes[i]._color.r, allShapes[i]._color.g, allShapes[i]._color.b),
                Type = (int) allShapes[i]._shapeType,
                OperationType = (int)Shape.Operation.None
            };
        }
        
        ComputeBuffer computeBuffer = new ComputeBuffer(allShapes.Count,ShapeData.GetSize());
        computeBuffer.SetData(allShapeData);
        _computeShader.SetBuffer(_kernelIndex,"_buffer",computeBuffer);
        _computeShader.SetInt("numberOfShapes",allShapes.Count);
        _computeBuffersToDispose.Add(computeBuffer);   
    }

    private void SetParameters()
    {
        _computeShader.SetMatrix("_CameraToWorld",_camera.cameraToWorldMatrix);
        _computeShader.SetMatrix("_CameraInverseProjection",_camera.projectionMatrix.inverse);
        _computeShader.SetVector("lightDirection",_light.transform.forward);
        _computeShader.SetFloat("softShadow", _enableSoftShadow);
    }
}

