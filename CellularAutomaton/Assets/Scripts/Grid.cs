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

    public Square(SquareType type , int rowIndex , int columnIndex)
    {
        Type = type;
        AdjacenctSquares = new AdjacencyInfo[8];
        AdjacenctSquares[0] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex - 1 }; //Top Left
        AdjacenctSquares[1] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex  }; //Top
        AdjacenctSquares[2] = new AdjacencyInfo() { Row = rowIndex - 1, Column = columnIndex + 1 }; //Top Right
        AdjacenctSquares[3] = new AdjacencyInfo() { Row = rowIndex , Column = columnIndex - 1 }; //Left
        AdjacenctSquares[4] = new AdjacencyInfo() { Row = rowIndex , Column = columnIndex + 1 }; //Right
        AdjacenctSquares[5] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex - 1 }; //Bottom Left
        AdjacenctSquares[6] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex - 1 }; //Bottom
        AdjacenctSquares[7] = new AdjacencyInfo() { Row = rowIndex + 1, Column = columnIndex + 1 }; //Bottom Right
    }
}

public class Grid
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public Square[,] AllSquares;

    public Grid( int rows , int columns)
    {
        Rows = rows;
        Columns = columns;

        AllSquares = new Square[Rows,Columns];
    }

    public void Generate(int noiseDensity)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int randomWeight = Random.Range(0, 100);
                if( randomWeight > noiseDensity)
                {
                    AllSquares[i, j] = new Square( SquareType.Floor , i , j );
                }
                else
                {
                    AllSquares[i, j] = new Square(SquareType.Wall, i, j);
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

    public SquareType ApplyCellularAutomaton( int rowIndex , int columnIndex)
    {
        Square square = AllSquares[rowIndex, columnIndex];
        int numberOfWalls = 0;
        int numberOfFloors = 0;
        
        for( int i = 0; i < 8; i++ )
        {
            AdjacencyInfo adjacencyInfo = square.AdjacenctSquares[i];

            if( IsIndexInBound(adjacencyInfo.Row,adjacencyInfo.Column))
            {
                if( GetSquareType(adjacencyInfo.Row,adjacencyInfo.Column) == SquareType.Wall)
                {
                    numberOfWalls++;
                }
                else
                {
                    numberOfFloors++;
                }
            }
            else
            {
                numberOfWalls++;
            }
        }

        if( numberOfWalls > 4 )
        {
            return SquareType.Wall;
        }
        if (numberOfWalls < 4)
        {
            return SquareType.Floor;
        }
        return square.Type;
    }
}

