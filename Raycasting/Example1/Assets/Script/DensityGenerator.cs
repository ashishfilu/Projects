using UnityEngine;

public class DensityGenerator
{
    protected ComputeShader _computeShader;
    protected int _kernelIndex;
    protected static int _threadSize = 8;//Should be same as [numthreads(8,8,8)] in ComputeShader

    public DensityGenerator()
    {
        _computeShader = Resources.Load("Shader/PlanarDensity") as ComputeShader;
        _kernelIndex = _computeShader.FindKernel("CSMain");
    }

    public virtual void Generate( Section section , ComputeBuffer pointsBuffer, float isoLevel)
    {
        float pointSpacing = (float)section.Size / (float)section.VoxelPerAxis;
        _computeShader.SetBuffer(_kernelIndex,"DataBuffer",pointsBuffer);

        _computeShader.SetFloat("Bound" , section.Size);
        _computeShader.SetFloat("Spacing" , pointSpacing);
        _computeShader.SetInt("PointsPerAxis" , section.PointsPerAxis);
        _computeShader.SetVector("StartPosition",section.StartPosition);

        int numberOfPoints = section.PointsPerAxis;
        int threadPerAxis = Mathf.CeilToInt ((float)numberOfPoints / (float)_threadSize);
        
        _computeShader.Dispatch(_kernelIndex,threadPerAxis,threadPerAxis,threadPerAxis);
    }
}
