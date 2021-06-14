using UnityEngine;

public class SphereDensity : DensityGenerator
{
    public SphereDensity() 
    {
        _computeShader = Resources.Load("Shader/SphereDensity") as ComputeShader;
        _kernelIndex = _computeShader.FindKernel("CSMain");
    }
    
    public override void Generate( Section section , ComputeBuffer pointsBuffer, float isoLevel)
    {
        _computeShader.SetFloat("Radius",isoLevel);
        base.Generate(section,pointsBuffer,isoLevel);
    }
}
