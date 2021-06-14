using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public int RowIndex, ColumnIndex;
    public Vector3 Position;

    public Cell(int rowIndex , int columnIndex , Vector3 position)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Position = position;
    }
}

public class Grid
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public Vector3 Scale { get; private set; }
    public Vector3 CellSize { get;private set; }

    public Cell[,] AllSquares;
    private Vector3 m_Position;
    

    public Grid( int rows , int columns , Vector3 gridSize , Vector3 position )
    {
        Rows = rows;
        Columns = columns;
        AllSquares = new Cell[Rows,Columns];

        m_Position = position;
        m_Position.y += 0.01f;

        Scale = new Vector3(gridSize.x , gridSize.y , gridSize.z );
        CellSize = new Vector3(gridSize.x / (float)Columns, gridSize.y, gridSize.z / (float)Rows);
    }

    public void Generate()
    {
        Vector3 startPosition = m_Position;
        startPosition.x -= Scale.x * 0.5f - CellSize.x * 0.5f;
        startPosition.z -= Scale.z * 0.5f - CellSize.z *0.5f;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Cell temp = new Cell( i, j , startPosition);
                AllSquares[i, j] = temp;
                startPosition.x += CellSize.x; ;
            }
            startPosition.z += CellSize.z  ;
            startPosition.x = m_Position.x - Scale.x * 0.5f + CellSize.x * 0.5f;
        }
    }

    public bool IsIndexInBound(int rowIndex , int columnIndex)
    {
        return rowIndex >= 0 && rowIndex < Rows && columnIndex >= 0 && columnIndex < Columns;
    }

    public Vector2 GetGridIndexFromPosition(Vector3 position)
    {
        float columnRatio = (position.x / Scale.x) + 0.5f;
        float rowRatio = (position.z / Scale.z) + 0.5f;

        float rowIndex = Rows * rowRatio;
        float columnIndex = Columns * columnRatio;

        return new Vector2((int)rowIndex,(int)columnIndex);
    }
}

