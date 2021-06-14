using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public int Rows;
    public int Columns;
    public GameObject GridRoot;
    public GameObject GameBoardRoot;
    public int Scale;
    [Range(0, 100)]
    public int NoiseDensity;
    [Range(0, 100)]
    public int Seed;

    private LevelRuntimeEntity m_GameLevel;
    private LevelViewModel m_GameLevelViewModel;
    
    void Start()
    {
        LevelData levelData = new LevelData();
        levelData.Rows = Rows;
        levelData.Columns = Columns;
        levelData.CellSize = Scale;
        levelData.Duration = 2.0f;
        levelData.NoiseDensity = NoiseDensity;
        levelData.Seed = Seed;

        m_GameLevel = new LevelRuntimeEntity(levelData);
        m_GameLevel.Initialize();

        GameObject levelView = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameLevel"));
        m_GameLevelViewModel = levelView.GetComponent<LevelViewModel>();
        m_GameLevelViewModel.SetData(m_GameLevel);
    }

    private void Update()
    {
        m_GameLevel.Update();
    }
}
