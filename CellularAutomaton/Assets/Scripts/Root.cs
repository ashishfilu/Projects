using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    private Grid mNoiseGrid;
    private Grid mOutputGrid;
    private List<SpriteRenderer> mGridVisualElements;
    private float mTime = 0.0f;
    private int mSimulationCount = 0;

    public GameObject VisualElementRoot;
    public float SimulationDelay;
    public int NumberOfSimulations;
    public int Rows;
    public int Columns;
    [Range(0,100)]
    public int NoiseDensity;
    // Start is called before the first frame update
    void Start()
    {
        mNoiseGrid = new Grid(Rows, Columns);
        mNoiseGrid.Generate(NoiseDensity);

        mOutputGrid = new Grid(Rows, Columns);
        mOutputGrid.Generate(-1);
        CopyNoiseGridToOutputGrid();

        InitializeGridVisualElements();
    }

    void InitializeGridVisualElements()
    {
        mGridVisualElements = new List<SpriteRenderer>();
        GameObject sourceSprite = Resources.Load("Prefabs/Grid")as GameObject;

        int positionX = -Columns / 2;
        int positionY = Rows / 2;

        for (int i = 0; i < Rows; i++)            
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject sprite = GameObject.Instantiate(sourceSprite);
                SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
                if( mOutputGrid.GetSquareType(i,j) == SquareType.Floor)
                {
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    spriteRenderer.color = new Color(0, 0, 0, 1);
                }
                mGridVisualElements.Add(spriteRenderer);
                sprite.transform.parent = VisualElementRoot.transform;
                sprite.transform.position = new Vector3(positionX, positionY, 0);
                positionX += 1;
            }
            positionY -= 1;
            positionX = -mNoiseGrid.Columns / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( mSimulationCount == NumberOfSimulations )
        {
            return;
        }

        mTime += Time.deltaTime;

        if (mTime > SimulationDelay)
        {
            mTime = 0;
            ApplyCellularAutoMaton();
            mSimulationCount++;
        }

        int index = 0;
        for (int j = 0; j < Columns; j++)
        {
            for (int i = 0; i < Rows; i++)
            {
                index = j * Rows + i;
                if (mOutputGrid.GetSquareType(i, j) == SquareType.Wall)
                {
                    mGridVisualElements[index].color = new Color(0, 0, 0, 1);
                }
                else
                {
                    mGridVisualElements[index].color = new Color(1, 1, 1, 1);
                }
            }
        }

        CopyOutputGridToNoiseGrid();
    }

    void ApplyCellularAutoMaton()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                mOutputGrid.AllSquares[i,j].Type = mNoiseGrid.ApplyCellularAutomaton(i, j);
            }
        }
    }

    void CopyOutputGridToNoiseGrid()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                mNoiseGrid.AllSquares[i, j].Type = mOutputGrid.AllSquares[i, j].Type;
            }
        }
    }

    void CopyNoiseGridToOutputGrid()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                mOutputGrid.AllSquares[i, j].Type = mNoiseGrid.AllSquares[i, j].Type;
            }
        }
    }
}
