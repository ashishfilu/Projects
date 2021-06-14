using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkLength = 5;
    public static readonly int ChunkHeight = 5;

    public static readonly int WorldSizeInChunk = 100;
    public static readonly int WorldSizeInVoxel = ChunkLength * WorldSizeInChunk ;
    public static readonly int ViewDistanceInChunk = 8;

    public static readonly float NormalizedTextureSize = 0.25f;


    public static readonly Vector3[] voxelVertices = new Vector3[8]
        {
            new Vector3(0,0,0), //0
            new Vector3(1,0,0), //1
            new Vector3(1,1,0), //2
            new Vector3(0,1,0), //3

            new Vector3(0,0,1), //4
            new Vector3(1,0,1), //5
            new Vector3(1,1,1), //6
            new Vector3(0,1,1), //7
        };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,-1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(0.0f,0.0f,-1.0f),
        new Vector3(-1.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
    };

    //Six faces and each face has 2 triangles . Each triangle needs 3 vertices so 6 vertices for each face . You can consider it as indices
    public static readonly int[,] voxelTriangles = new int[6, 4]
    {
        //Top,Bottom,Front,Back,Left,Right
        {3,7,2,6},//Top face
        {1,5,0,4},//Bottom face
        {5,6,4,7},//Front face
        {0,3,1,2},//Back Face
        {4,7,0,3},//Left face
        {1,2,5,6} //Right face
    };

    public static readonly Vector2[] uvs = new Vector2[4]
    {
        new Vector2(0.0f,0.0f),
        new Vector2(0.0f,1.0f),
        new Vector2(1.0f,0.0f),
        new Vector2(1.0f,1.0f)
    };

    public static readonly Vector2[] textureUVs = new Vector2[16]
    {
        new Vector2(0.0f,0.75f),
        new Vector2(0.25f,0.75f),
        new Vector2(0.5f,0.75f),
        new Vector2(0.75f,0.75f),

        new Vector2(0.0f,0.5f),
        new Vector2(0.25f,0.5f),
        new Vector2(0.5f,0.5f),
        new Vector2(0.75f,0.5f),

        new Vector2(0.0f,0.25f),
        new Vector2(0.25f,0.25f),
        new Vector2(0.5f,0.25f),
        new Vector2(0.75f,0.25f),

        new Vector2(0.0f,0.0f),
        new Vector2(0.25f,0.0f),
        new Vector2(0.5f,0.0f),
        new Vector2(0.75f,0.0f),
    };
}
