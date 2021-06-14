using System.Collections.Generic;
using UnityEngine;

public class MeshHelper
{
    //Given a Mesh/SkinnedMeshRenderer Vector3[] v = mesh.vertices, and int[] t = mesh.triangles.
    //For each vertex i in the mesh, find the set of neighboring vertices.
    //Computing the Laplacian Smooth Filter p[i] = ( 1 / number of adjacent vertices ) * summation of the adjacent vertices.
    
    public static Vector3[] LaplacianFilter(Vector3[] sv, int[] t)
    {
        Vector3[] wv = new Vector3[sv.Length];
        List<Vector3> adjacentVertices = new List<Vector3>();
 
        float dx = 0.0f;
        float dy = 0.0f;
        float dz = 0.0f;
 
        for (int vi=0; vi< sv.Length; vi++)
        {
            // Find the sv neighboring vertices
            adjacentVertices = FindAdjacentNeighbors (sv, t, sv[vi]);
 
            if (adjacentVertices.Count != 0)
            {
                dx = 0.0f;
                dy = 0.0f;
                dz = 0.0f;

                // Add the vertices and divide by the number of vertices
                for (int j=0; j<adjacentVertices.Count; j++)
                {
                    dx += adjacentVertices[j].x;
                    dy += adjacentVertices[j].y;
                    dz += adjacentVertices[j].z;
                }
 
                wv[vi].x = dx / adjacentVertices.Count;
                wv[vi].y = dy / adjacentVertices.Count;
                wv[vi].z = dz / adjacentVertices.Count;
            }
        }
 
        return wv;
    }
    
    public static List<Vector3> FindAdjacentNeighbors ( Vector3[] v, int[] t, Vector3 vertex )
    {
        List<Vector3>adjacentV = new List<Vector3>();
        List<int>facemarker = new List<int>();
        int facecount = 0;	
 
        // Find matching vertices
        for (int i=0; i<v.Length; i++)
            if (Mathf.Approximately (vertex.x, v[i].x) && 
                Mathf.Approximately (vertex.y, v[i].y) && 
                Mathf.Approximately (vertex.z, v[i].z))
            {
                int v1 = 0;
                int v2 = 0;
                bool marker = false;
 
                // Find vertex indices from the triangle array
                for(int k=0; k<t.Length; k=k+3)
                    if(facemarker.Contains(k) == false)
                    {
                        v1 = 0;
                        v2 = 0;
                        marker = false;
 
                        if(i == t[k])
                        {
                            v1 = t[k+1];
                            v2 = t[k+2];
                            marker = true;
                        }
 
                        if(i == t[k+1])
                        {
                            v1 = t[k];
                            v2 = t[k+2];
                            marker = true;
                        }
 
                        if(i == t[k+2])
                        {
                            v1 = t[k];
                            v2 = t[k+1];
                            marker = true;
                        }
 
                        facecount++;
                        if(marker)
                        {
                            // Once face has been used mark it so it does not get used again
                            facemarker.Add(k);
 
                            // Add non duplicate vertices to the list
                            if ( isVertexExist(adjacentV, v[v1]) == false )
                            {	
                                adjacentV.Add(v[v1]);
                            }
 
                            if ( isVertexExist(adjacentV, v[v2]) == false )
                            {
                                adjacentV.Add(v[v2]);
                            }
                            marker = false;
                        }
                    }
            }
 
        return adjacentV;
    }

    private static bool isVertexExist(List<Vector3> vertexList, Vector3 vertex)
    {
        bool marker = false;
        foreach (Vector3 vec in vertexList)
            if (Mathf.Approximately(vec.x,vertex.x) && Mathf.Approximately(vec.y,vertex.y) && Mathf.Approximately(vec.z,vertex.z))
            {
                marker = true;
                break;
            }
 
        return marker;
    }
}
