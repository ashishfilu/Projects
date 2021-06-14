using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkCoordinate
{
    public int X;
    public int Z;

    public ChunkCoordinate(int x , int z)
    {
        X = x;
        Z = z;
    }
}

public class Chunk 
{
    private GameObject m_GameObject;
    private MeshRenderer m_MeshRenderer;
    private MeshFilter m_MeshFilter;
    private ChunkCoordinate m_ChunkCoordinate;

    private int m_VertexCount = 0;
    private List<Vector3> m_Vertices;
    private List<int> m_Indices;
    private List<Vector2> m_UVs;
    private byte[,,] m_VoxelMap;
    private TerrainController m_TerrainController;

    public bool IsActive 
    {
        get
        {
            return m_GameObject.activeSelf;
        }
        set
        {
            m_GameObject.SetActive(value);
        }
    }

    public Vector3 Position
    {
        get
        {
            return m_GameObject.transform.position;
        }
    }

    public Chunk( ChunkCoordinate chunkCoordinate ,TerrainController terrainController )
    {
        m_TerrainController = terrainController;
        m_ChunkCoordinate = chunkCoordinate;
        m_GameObject = new GameObject($"Chunk_{m_ChunkCoordinate.X}_{m_ChunkCoordinate.Z}");
        m_MeshFilter = m_GameObject.AddComponent<MeshFilter>();
        m_MeshRenderer = m_GameObject.AddComponent<MeshRenderer>();
        m_MeshRenderer.material = terrainController.ChunkMaterial;

        m_GameObject.transform.parent = m_TerrainController.transform;
        m_GameObject.transform.position = new Vector3(m_ChunkCoordinate.X * VoxelData.ChunkLength, 0.0f, m_ChunkCoordinate.Z * VoxelData.ChunkLength);

        m_Vertices = new List<Vector3>();
        m_Indices = new List<int>();
        m_UVs = new List<Vector2>();
        m_VoxelMap = new byte[VoxelData.ChunkLength, VoxelData.ChunkHeight, VoxelData.ChunkLength];

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }
    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkLength; x++)
            {
                for (int z = 0; z < VoxelData.ChunkLength; z++)
                {
                    m_VoxelMap[x, y, z] = m_TerrainController.GetVoxel(new Vector3(x, y, z) + Position);
                }
            }
        }
    }

    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkLength; x++)
            {
                for (int z = 0; z < VoxelData.ChunkLength; z++)
                {
                    AddChunkToVoxel(new Vector3(x, y, z));
                }
            }
        }
    }

    private void AddChunkToVoxel(Vector3 position)
    {
        for (int faceindex = 0; faceindex < 6; faceindex++)
        {
            if (!CheckVoxel(position + VoxelData.faceChecks[faceindex]))
            {
                byte blockId = m_VoxelMap[(int)position.x, (int)position.y, (int)position.z];

                m_Vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[faceindex, 0]] + position);
                m_Vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[faceindex, 1]] + position);
                m_Vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[faceindex, 2]] + position);
                m_Vertices.Add(VoxelData.voxelVertices[VoxelData.voxelTriangles[faceindex, 3]] + position);

                m_Indices.Add(m_VertexCount);
                m_Indices.Add(m_VertexCount + 1);
                m_Indices.Add(m_VertexCount + 2);
                m_Indices.Add(m_VertexCount + 2);
                m_Indices.Add(m_VertexCount + 1);
                m_Indices.Add(m_VertexCount + 3);

                int textureId = m_TerrainController.BlockTypes[blockId].GetTextureId(faceindex);

                AddTexture(textureId);
                m_VertexCount += 4;
            }
        }
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Indices.ToArray();
        mesh.uv = m_UVs.ToArray();
        mesh.RecalculateNormals();
        m_MeshFilter.mesh = mesh;
    }

    private bool CheckVoxel(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int z = Mathf.FloorToInt(position.z);

        bool voxelInChunk = IsVoxelInChunk(x, y, z);
        if( voxelInChunk )
        {
            return m_TerrainController.BlockTypes[m_VoxelMap[x,y,z]].IsSolid;
        }
        return m_TerrainController.BlockTypes[m_TerrainController.GetVoxel(position+Position)].IsSolid;
    }

    private void AddTexture(int textureId)
    {
        Vector2 uv = VoxelData.textureUVs[textureId];
        m_UVs.Add(uv);
        m_UVs.Add(uv + new Vector2(0, VoxelData.NormalizedTextureSize));
        m_UVs.Add(uv + new Vector2(VoxelData.NormalizedTextureSize, 0));
        m_UVs.Add(uv + new Vector2(VoxelData.NormalizedTextureSize, VoxelData.NormalizedTextureSize));
    }

    public bool IsVoxelInChunk( int x , int y , int z )
    {
        if (x < 0 || x > VoxelData.ChunkLength - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkLength - 1)
        {
            return false;
        }
        return true;
    }
}
