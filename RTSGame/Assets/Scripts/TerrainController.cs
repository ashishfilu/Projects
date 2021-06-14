using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BlockType
{
    public string Name;
    public bool IsSolid;

    //Top,Bottom,Front,Back,Left,Right
    [Header("Texture Data")]
    public int Top;
    public int Bottom;
    public int Front;
    public int Back;
    public int Left;
    public int Right;

    public int GetTextureId( int faceId )
    {
        switch( faceId )
        {
            case 0:
                return Top;
            case 1:
                return Bottom;
            case 2:
                return Front;
            case 3:
                return Back;
            case 4:
                return Left;
            case 5:
                return Right;
            default:
                return -1;
        }
    }
}

public class TerrainController : MonoBehaviour
{
    public GameObject   Plane;
    public CameraController m_Camera;
    public BlockType[]  BlockTypes;
    public Material     ChunkMaterial;

    private Chunk[,] m_ChunkMap;
    private Vector3 m_SpawnPosition;

    private void Start()
    {
        m_ChunkMap = new Chunk[VoxelData.WorldSizeInChunk, VoxelData.WorldSizeInChunk];
        m_SpawnPosition = new Vector3(VoxelData.WorldSizeInChunk / 2 * VoxelData.ChunkLength, VoxelData.ChunkHeight, VoxelData.WorldSizeInChunk / 2 * VoxelData.ChunkLength);
        Generate();
    }

    private void Generate()
    {
        for( int x = (VoxelData.WorldSizeInChunk/2 ) - VoxelData.ViewDistanceInChunk; x < (VoxelData.WorldSizeInChunk/2) + VoxelData.ViewDistanceInChunk ; x++ )
        {
            for ( int z = (VoxelData.WorldSizeInChunk/2) - VoxelData.ViewDistanceInChunk ; z < (VoxelData.WorldSizeInChunk/2) + VoxelData.ViewDistanceInChunk; z++)
            {
                CreateNewChunk(x, z);
            }
        }

        m_Camera.transform.position = m_SpawnPosition;
        m_Camera.Reset();
    }

    private void CreateNewChunk( int x , int z)
    {
        m_ChunkMap[x, z] = new Chunk(new ChunkCoordinate(x, z), this);
    }

    private void Update()
    {
        
    }

    public bool IsChunkInWorld(ChunkCoordinate chunkCoordinate)
    {
        if( chunkCoordinate.X > 0 && chunkCoordinate.X < VoxelData.WorldSizeInChunk-1
            && chunkCoordinate.Z > 0 && chunkCoordinate.Z < VoxelData.WorldSizeInChunk-1)
        {
            return true;
        }
        return false;
    }

    public bool IsVoxelInWorld( Vector3 position)
    {
        if( position.x >= 0 && position.x < VoxelData.WorldSizeInVoxel
            && position.y >= 0 && position.y < VoxelData.ChunkHeight
            && position.z >= 0 && position.z < VoxelData.WorldSizeInVoxel )
        {
            return true;
        }
        return false;
    }

    public byte GetVoxel( Vector3 position )
    {
        if( !IsVoxelInWorld(position))
        {
            return 0;
        }
        if (position.y == 0)
        {
            return 2;
        }
        else if (position.y == VoxelData.ChunkHeight - 1)
        {
            return 1;
        }
        else
        {
            return 3;
        }
    }
}


