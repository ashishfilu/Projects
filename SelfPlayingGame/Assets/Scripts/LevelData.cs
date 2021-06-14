using System.Collections.Generic;

[System.Serializable]
public struct EntityData
{
    public int RowIndex;
    public int ColumnIndex;
    public EntityType OccupierType;
}

[System.Serializable]
public struct LevelData 
{
    public int Rows;
    public int Columns;
    public int CellSize;
    public float Duration;
    public int NoiseDensity;
    public int Seed;

    public List<EntityData> EntityList;

    public LevelData( int rows , int columns , int cellSize , float duration , int noiseDensity , int seed )
    {
        Rows = rows;
        Columns = columns;
        CellSize = cellSize;
        Duration = duration;
        NoiseDensity = noiseDensity;
        Seed = seed;
        EntityList = new List<EntityData>();
    }

    public void AddEntity( EntityData entityData )
    {
        EntityList.Add(entityData);
    }
}
