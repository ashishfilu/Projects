using System.Collections.Generic;
using UnityEngine;

public enum SquareType
{
    Floor,
    Wall
}

public struct AdjacencyInfo
{
    public int Row;
    public int Column;
}

public class Square
{
    public SquareType Type;
    public AdjacencyInfo[] AdjacenctSquares;
    public int RowIndex, ColumnIndex;
    public EntityType OccupierType;

    public Square(SquareType type , int rowIndex , int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Type = type;
        OccupierType = EntityType.NONE;
        AdjacenctSquares = new AdjacencyInfo[8];
        AdjacenctSquares[0] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex - 1 }; //Top Left
        AdjacenctSquares[1] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex  }; //Top
        AdjacenctSquares[2] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex + 1 }; //Top Right
        AdjacenctSquares[3] = new AdjacencyInfo() { Row = rowIndex , Column = columnIndex - 1 }; //Left
        AdjacenctSquares[4] = new AdjacencyInfo() { Row = rowIndex , Column = columnIndex + 1 }; //Right
        AdjacenctSquares[5] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex - 1 }; //Bottom Left
        AdjacenctSquares[6] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex }; //Bottom
        AdjacenctSquares[7] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex + 1 }; //Bottom Right
    }
}

public class Grid
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public int Scale { get; set; }

    public Square[,] AllSquares;

    public List<Square> FloorSquares { get; set; }

    public List<Square> WallSquares { get; set; }

    public Grid( int rows , int columns , int scale )
    {
        Rows = rows;
        Columns = columns;
        Scale = scale;
        AllSquares = new Square[Rows,Columns];
        FloorSquares = new List<Square>();
        WallSquares = new List<Square>();
    }

    public void Generate(int noiseDensity , int seed )
    {
        System.Random random = new System.Random(seed);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int randomWeight = random.Next(0, 100);
                if( randomWeight > noiseDensity)
                {
                    Square temp = new Square(SquareType.Floor, i, j);
                    AllSquares[i, j] = temp;
                    FloorSquares.Add(temp);
                }
                else
                {
                    Square temp = new Square(SquareType.Wall, i, j);
                    AllSquares[i, j] = temp;
                    WallSquares.Add(temp);
                }
            }
        }
    }


    public SquareType GetSquareType(int rowIndex , int columnIndex)
    {
        return AllSquares[rowIndex, columnIndex].Type;
    }

    public bool IsIndexInBound(int rowIndex , int columnIndex)
    {
        return rowIndex >= 0 && rowIndex < Rows && columnIndex >= 0 && columnIndex < Columns;
    }

    public Vector3 GetPositionFromGridIndex(int row, int column)
    {
        Vector3 output = Vector3.one;
        output.x = -((Columns -1 )* Scale * 0.5f) + (column * Scale);
        output.y = ( (Rows -1 )* Scale * 0.5f) - (row * Scale);
        return output;
    }

    public Vector2 GetGridIndexFromPosition(Vector3 position)
    {
        float columnRatio = (position.x / (Columns-1)) * 0.5f + 0.5f;
        float rowRatio = -(position.y / (Rows-1)) * 0.5f + 0.5f;

        return new Vector2((int)Mathf.Lerp(0, Rows, rowRatio), (int)Mathf.Lerp(0, Columns, columnRatio));
    }

    public void SetOccupierType( int row , int column , EntityType occupierType )
    {
        if( IsIndexInBound(row,column))
        {
            AllSquares[row, column].OccupierType = occupierType;
        }
    }

    public EntityType GetOccupierType( int row , int column)
    {
        if( IsIndexInBound(row,column))
        {
            return AllSquares[row, column].OccupierType;
        }
        return EntityType.NONE;
    }
}

