using UnityEngine;

public class LevelViewModel : MonoBehaviour
{
    public GameObject GridRoot;
    public GameObject EntitiesRoot;

    private Player m_Player;
    private LevelRuntimeEntity m_LevelEntity;
    private Grid m_Grid;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGridVisualElements();
        InitializePlayer();
        InitializeControls();
    }

    public void SetData(LevelRuntimeEntity levelRuntimeEntity)
    {
        m_LevelEntity = levelRuntimeEntity;
        m_Grid = m_LevelEntity.LevelGrid;
    }

    void InitializeGridVisualElements()
    {
        GameObject sourceSprite = Resources.Load("Prefabs/Grid") as GameObject;
        LevelData levelData = m_LevelEntity.Data;

        int columns = levelData.Columns;
        int rows = levelData.Rows;
        int cellSize = levelData.CellSize;

        int positionX = -(columns - 1) * (cellSize / 2);
        int positionY = (rows - 1) * (cellSize / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
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
                sprite.transform.localScale = new Vector3(cellSize, cellSize, 1);
                sprite.transform.parent = GridRoot.transform;
                sprite.transform.position = new Vector3(positionX, positionY, 0);
                positionX += cellSize;
            }
            positionY -= cellSize;
            positionX = -(columns - 1) * (cellSize / 2);
        }
    }

    void InitializePlayer()
    {
        GameObject player = GameRoot.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        player.transform.parent = EntitiesRoot.transform;
        Square randomFloorSqaure = m_Grid.FloorSquares[43/*Random.Range(0, m_Grid.FloorSquares.Count - 1)*/];

        m_Player = player.GetComponent<Player>();
        m_Player.GameBoard = m_Grid;
        m_Player.GridIndex = new Vector2(randomFloorSqaure.RowIndex, randomFloorSqaure.ColumnIndex);
        m_Player.InitializePlayerState();
        m_Grid.FloorSquares.Remove(randomFloorSqaure);
    }

    void InitializeControls()
    {
        int rotatorCount = Random.Range(1, 1);
        GameObject rotator = Resources.Load<GameObject>("Prefabs/Rotator");
        for (int i = 0; i < rotatorCount; i++)
        {
            GameObject rotatorInstance = GameObject.Instantiate(rotator);
            Rotator rotatorScript = rotatorInstance.GetComponent<Rotator>();

            rotatorInstance.transform.parent = EntitiesRoot.transform;

            Square randomFloorSqaure = m_Grid.FloorSquares[2/*Random.Range(0, m_Grid.FloorSquares.Count - 1)*/];
            rotatorInstance.transform.localPosition = m_Grid.GetPositionFromGridIndex(randomFloorSqaure.RowIndex, randomFloorSqaure.ColumnIndex);
            rotatorInstance.transform.localScale = Vector3.one * m_LevelEntity.Data.CellSize;

            m_Grid.SetOccupierType(randomFloorSqaure.RowIndex, randomFloorSqaure.ColumnIndex, rotatorScript.Direction == RotationDirection.CCW ? EntityType.ROTATE_CCW : EntityType.ROTATE_CW);
            m_Grid.FloorSquares.Remove(randomFloorSqaure);
        }

        for (int i = 0; i < rotatorCount; i++)
        {
            GameObject rotatorInstance = GameObject.Instantiate(rotator);
            Rotator rotatorScript = rotatorInstance.GetComponent<Rotator>();

            rotatorInstance.transform.parent = EntitiesRoot.transform;

            Square randomFloorSqaure = m_Grid.FloorSquares[3/*Random.Range(0, m_Grid.FloorSquares.Count - 1)*/];
            rotatorInstance.transform.localPosition = m_Grid.GetPositionFromGridIndex(randomFloorSqaure.RowIndex, randomFloorSqaure.ColumnIndex);
            rotatorInstance.transform.localScale = Vector3.one * m_LevelEntity.Data.CellSize;

            m_Grid.SetOccupierType(randomFloorSqaure.RowIndex, randomFloorSqaure.ColumnIndex, rotatorScript.Direction == RotationDirection.CCW ? EntityType.ROTATE_CCW : EntityType.ROTATE_CW);
            m_Grid.FloorSquares.Remove(randomFloorSqaure);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
