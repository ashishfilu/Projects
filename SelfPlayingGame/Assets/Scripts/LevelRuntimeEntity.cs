using UnityEngine;

public class LevelRuntimeEntity
{
    public LevelData Data { get; private set; }
    public Grid LevelGrid { get; private set; }
    public float Duration { get; private set; }

    public LevelRuntimeEntity(LevelData levelData)
    {
        Data = levelData;
        Duration = levelData.Duration;
    }

    public void Initialize()
    {
        LevelGrid = new Grid(Data.Rows, Data.Columns, Data.CellSize);
        LevelGrid.Generate(Data.NoiseDensity, Data.Seed);
    }
    
    public void Update()
    {
        Duration -= Time.deltaTime;
    }
}
