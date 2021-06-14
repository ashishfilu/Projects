using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SceneEditor : MonoBehaviour
{
    public int Rows;
    public int Columns;
    public GameObject GridRoot;
    public GameObject GameBoardRoot;
    public int Scale;
    [Range(0, 100)]
    public int NoiseDensity;
    [Range(0, 200)]
    public int Seed = 100;

    private Grid m_Grid;
    private Player m_Player;
    private EntityType m_ControlTypeToSpawn;
    private List<LevelData> m_AllLevel;

    void Start()
    {
        m_Grid = new Grid(Rows, Columns, Scale);
        m_Grid.Generate(NoiseDensity, Seed);
        m_ControlTypeToSpawn = EntityType.NONE;
#if UNITY_EDITOR
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Levels/AllLevels.json");
        if( textAsset == null )
        {
            m_AllLevel = new List<LevelData>();
        }
        else
        {
            m_AllLevel = JsonUtility.FromJson<List<LevelData>>(textAsset.text);
        }
#endif
        InitializeGridVisualElements();
        RegisterEvents();
    }

    public void LoadLevel( LevelData levelData )
    {
        Rows = levelData.Rows;
        Columns = levelData.Columns;
        Scale = levelData.CellSize;
        Seed = levelData.Seed;
        NoiseDensity = levelData.NoiseDensity;

        m_Grid = new Grid(levelData.Rows, levelData.Columns, levelData.CellSize);
        m_Grid.Generate(levelData.NoiseDensity, levelData.Seed);
        m_ControlTypeToSpawn = EntityType.NONE;

        int childCount = GridRoot.transform.childCount;
        for( int index = childCount - 1; index >= 0; index-- )
        {
            GameObject child = GridRoot.transform.GetChild(index).gameObject;
            GameObject.Destroy(child);
        }

        InitializeGridVisualElements();
        for( int i = 0; i < levelData.EntityList.Count ; i++ )
        {
            EntityData entityData = levelData.EntityList[i];

            switch (entityData.OccupierType)
            {
                case EntityType.SPAWN:
                    InitializePlayer(entityData.RowIndex, entityData.ColumnIndex);
                    break;
                case EntityType.BOT:
                    InitializeBot(entityData.RowIndex, entityData.ColumnIndex);
                    break;
                case EntityType.ROTATE_CCW:
                case EntityType.ROTATE_CW:
                    InitializeControls(entityData.RowIndex, entityData.ColumnIndex, entityData.OccupierType);
                    break;
            }
        }
    }

    void InitializeGridVisualElements()
    {
        GameObject sourceSprite = Resources.Load("Prefabs/Grid") as GameObject;

        int positionX = -(Columns - 1) * (Scale / 2);
        int positionY = (Rows - 1) * (Scale / 2);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject sprite = GameObject.Instantiate(sourceSprite);
                SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
                if (m_Grid.GetSquareType(i, j) == SquareType.Floor)
                {
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    spriteRenderer.color = new Color(0, 0, 0, 1);
                }
                sprite.transform.localScale = new Vector3(Scale, Scale, 1);
                sprite.transform.parent = GridRoot.transform;
                sprite.transform.position = new Vector3(positionX, positionY, 0);
                positionX += Scale;
            }
            positionY -= Scale;
            positionX = -(Columns - 1) * (Scale / 2);
        }
    }

    private void Update()
    {
        if( m_ControlTypeToSpawn != EntityType.NONE )
        {
            if( Input.GetMouseButtonDown(0) && !(EventSystem.current.IsPointerOverGameObject()))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 gridIndex = m_Grid.GetGridIndexFromPosition(mousePosition);

                switch(m_ControlTypeToSpawn)
                {
                    case EntityType.SPAWN:
                        InitializePlayer((int)gridIndex.x, (int)gridIndex.y);
                        break;
                    case EntityType.BOT:
                        InitializeBot((int)gridIndex.x, (int)gridIndex.y);
                        break;
                    case EntityType.ROTATE_CCW:
                    case EntityType.ROTATE_CW:
                        InitializeControls((int)gridIndex.x, (int)gridIndex.y, m_ControlTypeToSpawn);
                        break;
                }
                
            }
        }
    }

    void InitializePlayer(int row,int column)
    {
        GameObject player = GameRoot.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        player.transform.parent = GameBoardRoot.transform;
       
        m_Player = player.GetComponent<Player>();
        m_Player.GameBoard = m_Grid;
        m_Player.GridIndex = new Vector2(row,column);

        m_Grid.SetOccupierType(row, column, EntityType.SPAWN);
    }

    void InitializeBot(int row, int column)
    {
        GameObject bot = GameRoot.Instantiate(Resources.Load<GameObject>("Prefabs/Bot"));
        bot.transform.parent = GameBoardRoot.transform;

        bot.transform.localPosition = m_Grid.GetPositionFromGridIndex(row, column);
        bot.transform.localScale = Vector3.one * Scale;

        m_Grid.SetOccupierType(row, column, EntityType.BOT);

    }

    void InitializeControls(int row , int column , EntityType controlType)
    {
        GameObject rotator = Resources.Load<GameObject>("Prefabs/Rotator");
        GameObject rotatorInstance = GameObject.Instantiate(rotator);
        Rotator rotatorScript = rotatorInstance.GetComponent<Rotator>();

        if( controlType == EntityType.ROTATE_CW )
        {
            rotatorScript.Direction = RotationDirection.CW;
        }
        else
        {
            rotatorScript.Direction = RotationDirection.CCW;
        }

        rotatorInstance.transform.parent = GameBoardRoot.transform;

        rotatorInstance.transform.localPosition = m_Grid.GetPositionFromGridIndex(row,column);
        rotatorInstance.transform.localScale = Vector3.one * Scale;

        m_Grid.SetOccupierType(row, column, controlType);
    }

    void RegisterEvents()
    {
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnSpawnerButtonClicked, OnSpawnerButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnRotatorCWButtonClicked, OnRotatorCWButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnRotatorCCWButtonClicked, OnRotatorCCWButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnAIBotButtonClicked, OnAIButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnLoadButtonClicked, OnLoadButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnSaveButtonClicked, OnSaveButtonClicked);
        GameEventManager.Instance.SubscribeEventListener(GameEventIDs.OnSimulateButtonClicked, OnSimulateButtonClicked);
    }

    private void OnSpawnerButtonClicked(System.Object triggerId)
    {
        m_ControlTypeToSpawn = EntityType.SPAWN;
    }
    private void OnRotatorCWButtonClicked(System.Object triggerId)
    {
        m_ControlTypeToSpawn = EntityType.ROTATE_CW;
    }
    private void OnRotatorCCWButtonClicked(System.Object triggerId)
    {
        m_ControlTypeToSpawn = EntityType.ROTATE_CCW;
    }
    private void OnAIButtonClicked(System.Object triggerId)
    {
        m_ControlTypeToSpawn = EntityType.BOT;
    }
    private void OnLoadButtonClicked(System.Object triggerId)
    {
#if UNITY_EDITOR
        string filePath = EditorUtility.OpenFilePanel("Choose Level", "Assets/Resources/Levels", "json");
        if( filePath.Length > 0 )
        {
            filePath = filePath.Substring(filePath.IndexOf("Assets"));
            TextAsset jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            if( jsonAsset != null )
            {
                LevelData levelData = JsonUtility.FromJson<LevelData>(jsonAsset.text);
                LoadLevel(levelData);
            }
        }
#endif
    }
    private void OnSaveButtonClicked(System.Object triggerId)
    {
#if UNITY_EDITOR
        LevelData levelData = new LevelData(m_Grid.Rows, m_Grid.Columns, Scale, 10.0f, NoiseDensity, Seed);
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                EntityType entityType = m_Grid.GetOccupierType(row, column);
                if (entityType != EntityType.NONE)
                {
                    EntityData temp = new EntityData();
                    temp.RowIndex = row;
                    temp.ColumnIndex = column;
                    temp.OccupierType = entityType;
                    levelData.AddEntity(temp);
                }
            }
        }

        string jsonString = JsonUtility.ToJson(levelData, true);

        string filePath = EditorUtility.SaveFilePanel("save Level", "Assets/Resources/Levels" , "Level" , "json");

        if (filePath.Length > 0)
        {
            filePath = filePath.Substring(filePath.IndexOf("Assets"));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(jsonString);
                }
            }
            AssetDatabase.Refresh();
            
        }
#endif
    }
    private void OnSimulateButtonClicked(System.Object triggerId)
    {

    }
}
