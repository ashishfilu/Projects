using UnityEngine;

public class NoiseDensity : DensityGenerator
{
    private int _seed = 6;
    private int _numOctaves = 6;
    private float _lacunarity = 2;
    private float _persistence = .5f;
    private float _noiseScale = 3f;
    private float _noiseWeight = 6f;
    private float _floorOffset = 2f;
    private float _weightMultiplier = 4f;

    private float _hardFloorHeight = -3f;
    private float _hardFloorWeight = 3f;

    private Vector4 _shaderParams = new Vector4(1,0,0,0);
    
    public NoiseDensity() 
    {
        _computeShader = Resources.Load("Shader/NoiseDensity") as ComputeShader;
        _kernelIndex = _computeShader.FindKernel("CSMain");
    }
    
    public override void Generate( Section section , ComputeBuffer pointsBuffer, float isoLevel)
    {
        var prng = new System.Random (_seed);
        var offsets = new Vector3[_numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < _numOctaves; i++) {
            offsets[i] = new Vector3 ((float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1, (float) prng.NextDouble () * 2 - 1) * offsetRange;
        }
        
        var offsetsBuffer = new ComputeBuffer (offsets.Length, sizeof (float) * 3);
        offsetsBuffer.SetData (offsets);

        _computeShader.SetVector ("centre", section.StartPosition);
        _computeShader.SetInt ("octaves", Mathf.Max (1, _numOctaves));
        _computeShader.SetFloat ("lacunarity", _lacunarity);
        _computeShader.SetFloat ("persistence", _persistence);
        _computeShader.SetFloat ("noiseScale", _noiseScale);
        _computeShader.SetFloat ("noiseWeight", _noiseWeight);
        _computeShader.SetBuffer (0, "offsets", offsetsBuffer);
        _computeShader.SetFloat ("floorOffset", _floorOffset);
        _computeShader.SetFloat ("weightMultiplier", _weightMultiplier);
        _computeShader.SetFloat ("hardFloor", _hardFloorHeight);
        _computeShader.SetFloat ("hardFloorWeight", _hardFloorWeight);
        _computeShader.SetVector("offset",new Vector3(-0.64f,0,0));

        _computeShader.SetVector ("params", _shaderParams);
        
        base.Generate(section,pointsBuffer,isoLevel);
        
        offsetsBuffer.Release();
    }
}
