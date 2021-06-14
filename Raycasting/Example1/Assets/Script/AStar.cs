using System.Collections.Generic;
using UnityEngine;

public enum HeuristicType
{
    Manhattan,
    Diagonal,
    Euclidean,
}
//https://www.geeksforgeeks.org/a-search-algorithm/

public class AStar:Singleton<AStar>
{
    public void GetPath(Vector3 gridStartPoint, Vector3 currentPosition ,
                                 Vector3 targetPoint,int numberOfGrids,Vector2 gridSize, 
                                List<Grid> grids, ref List<Vector3> outputPath ,  HeuristicType type = HeuristicType.Euclidean )
    {
        
        int x = Mathf.CeilToInt(Mathf.Abs(currentPosition.x - gridStartPoint.x ) / gridSize.x)-1;
        int z = Mathf.CeilToInt(Mathf.Abs(currentPosition.z - gridStartPoint.z ) / gridSize.y)-1;
        int index = z * numberOfGrids + x;
        Grid startGrid = grids[index];
        Debug.Log($"Start:{x},{z}");
        
        x = Mathf.CeilToInt(Mathf.Abs(targetPoint.x - gridStartPoint.x ) / gridSize.x)-1;
        z = Mathf.CeilToInt(Mathf.Abs(targetPoint.z - gridStartPoint.z ) / gridSize.y)-1;
        index = z * numberOfGrids + x;
        Grid targetGrid = grids[index];
        Debug.Log($"Target:{x},{z}");

        if (startGrid.Active == false || targetGrid.Active == false)
        {
            outputPath.Clear();
            return;
        }
        Perform_A_Star(startGrid, targetGrid, grids, numberOfGrids,type,ref outputPath);
        //Add current position of player
        outputPath.Add(currentPosition);
    }
    
    private void Perform_A_Star(Grid start , Grid end , List<Grid> grids , int numberOfGrids,
                                HeuristicType type , ref List<Vector3> outputPath )
    {
        outputPath.Clear();
        int maxIteration = numberOfGrids * numberOfGrids;
        int currentIteration = 0;
        
        List<Grid> openList = new List<Grid>();
        List<Grid> closedList = new List<Grid>();

        start.G = start.H = start.F = 0.0f;//Initialize start grid
        openList.Add(start);
        
        List<Grid> neighbors = new List<Grid>();

        while (openList.Count > 0)
        {
            //Choose grid with lowest F value . Remove from OpenList and add to close list.
            float minF = float.MaxValue;
            int index = -1;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].F < minF)
                {
                    minF = openList[i].F;
                    index = i;
                }
            }
            
            Grid currentGrid = openList[index];
            openList.RemoveAt(index);
            
            closedList.Add(currentGrid);

            if (currentGrid == end)
            {
                break;
            }
            
            //Calculate all neighbors
            neighbors.Clear();
            int leftIndex = currentGrid.IndexZ * numberOfGrids + (currentGrid.IndexX - 1);
            int rightIndex = currentGrid.IndexZ * numberOfGrids + (currentGrid.IndexX + 1);
            int topIndex = (currentGrid.IndexZ + 1) * numberOfGrids + currentGrid.IndexX;
            int bottomIndex = (currentGrid.IndexZ - 1) * numberOfGrids + currentGrid.IndexX;
            
            int topLeftIndex = (currentGrid.IndexZ+1) * numberOfGrids + (currentGrid.IndexX - 1);
            int topRightIndex = (currentGrid.IndexZ+1) * numberOfGrids + (currentGrid.IndexX + 1);
            int bottomLeftIndex = (currentGrid.IndexZ - 1) * numberOfGrids + (currentGrid.IndexX-1);
            int bottomRightIndex = (currentGrid.IndexZ - 1) * numberOfGrids + (currentGrid.IndexX+1);
            
            if ( leftIndex >= 0 && leftIndex < grids.Count )
            {
                neighbors.Add( grids[leftIndex]);
            }
            if (rightIndex >= 0 && rightIndex < grids.Count )
            {
                neighbors.Add(grids[rightIndex]);
            }
            if (topIndex >= 0 && topIndex < grids.Count )
            {
                neighbors.Add(grids[topIndex]);
            }
            if (bottomIndex >= 0 && bottomIndex < grids.Count )
            {
                neighbors.Add(grids[bottomIndex]);
            }
            
            if ( topLeftIndex >= 0 && topLeftIndex < grids.Count )
            {
                neighbors.Add(grids[topLeftIndex]);
            }
            if (topRightIndex >= 0 && topRightIndex < grids.Count )
            {
                neighbors.Add(grids[topRightIndex]);
            }
            if (bottomLeftIndex >= 0 && bottomLeftIndex < grids.Count )
            {
                neighbors.Add(grids[bottomLeftIndex]);
            }
            if (bottomRightIndex >= 0 && bottomRightIndex < grids.Count )
            {
                neighbors.Add(grids[bottomRightIndex]);
            }            
           
            //Iterate through all neighbors and update F,G and H
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!neighbors[i].Active)
                {
                    continue;
                }
                
                float G = currentGrid.G + ( neighbors[i].Position - currentGrid.Position ).magnitude;
                float H = 0;
                
                if (type == HeuristicType.Euclidean)
                {
                    H = CalculateEuclidean(end.Position, neighbors[i].Position);   
                }
                else if (type == HeuristicType.Diagonal)
                {
                    H = CalculateDiagonal(end.Position, neighbors[i].Position);   
                }
                if (type == HeuristicType.Manhattan)
                {
                    H = CalculateManhattan(end.Position, neighbors[i].Position);   
                }
                float F = G + H;

                //if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
                //remove neighbor from CLOSED
                if (closedList.Contains(neighbors[i]) && F < neighbors[i].G )
                {
                    closedList.Remove(neighbors[i]);
                }
                //if neighbor in OPEN and cost less than g(neighbor):
                //remove neighbor from OPEN, because new path is better
                if (openList.Contains(neighbors[i]) && F < neighbors[i].G )
                {
                    openList.Remove(neighbors[i]);
                }
                
                if ( closedList.Contains(neighbors[i]) == false &&
                     openList.Contains(neighbors[i]) == false )
                {
                    neighbors[i].G = G;
                    neighbors[i].H = H;
                    neighbors[i].F = F;
                    neighbors[i].Parent = currentGrid;
                    openList.Add( neighbors[i]);
                }
            }

            currentIteration++;
            if (currentIteration > maxIteration)
            {
                break;
            }
        }

        Grid iterator = end;
        while (iterator != null)
        {
            outputPath.Add(iterator.Position);
            iterator = iterator.Parent;
        }

        openList.Clear();
        closedList.Clear();
    }

    private float CalculateManhattan(Vector3 p1, Vector3 p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.z - p2.z);
    }
    private float CalculateDiagonal(Vector3 p1, Vector3 p2)
    {
        return Mathf.Max(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.z - p2.z));
    }
    private float CalculateEuclidean(Vector3 p1, Vector3 p2)
    {
        return (p1 - p2).magnitude;
    }
}