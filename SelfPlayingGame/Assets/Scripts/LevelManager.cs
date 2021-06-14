using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private LevelData[] m_AllLevels;
    public int NumberOfLevels { get; set; }

    public void Initialize()
    {
        
    }
    
    private void LoadAllLevels()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Levels/AllLevels");
        if( textAsset != null )
        {
            string[] allFiles = JsonUtility.FromJson<string[]>(textAsset.text);
            if( allFiles != null )
            {
                m_AllLevels = new LevelData[allFiles.Length];
                for (int i = 0; i < allFiles.Length; i++)
                {
                    textAsset = Resources.Load<TextAsset>(allFiles[i]);
                    if( textAsset != null )
                    {
                        //LevelData temp = 
                    }
                }
            }

            m_AllLevels = JsonUtility.FromJson<LevelData[]>(textAsset.text);
            NumberOfLevels = m_AllLevels.Length;
        }
    }

    public LevelData GetLevelData( int index )
    {
        return m_AllLevels[index];
    }
}
