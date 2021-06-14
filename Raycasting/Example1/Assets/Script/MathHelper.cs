using UnityEngine;

public class MathHelper
{
    public static void CheckLineIntersection2D(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out bool intersect,
        out Vector3 point)
    {
        point = Vector3.negativeInfinity;
        intersect = false;
        //https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
        float denominator = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);

        if (denominator == 0.0f)
        {
            return;
        }
        
        float t = ((p1.x - p3.x) * (p3.y - p4.y) - (p1.y - p3.y) * (p3.x - p4.x)) / denominator;
        float u = -((p1.x - p2.x) * (p1.y - p3.y) - (p1.y - p2.y) * (p1.x - p3.x)) / denominator;

        if ( 0.0f < u && u < 1.0f && t > 0.0f )
        {
            intersect = true;
            point.x = p1.x + t * (p2.x - p1.x);
            point.y = p1.y + t * (p2.y - p1.y);
            point.z = 0;
        }
    }

    public static void GetMinMaxPoints(Vector3[] array, out Vector3 min, out Vector3 max)
    {
        min = Vector3.positiveInfinity;
        max = Vector3.negativeInfinity;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].x < min.x )
            {
                min.x = array[i].x;
            }
            if (array[i].y < min.y )
            {
                min.y = array[i].y;
            }
            if (array[i].z < min.z )
            {
                min.z = array[i].z;
            }
        }
        
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].x > max.x )
            {
                max.x = array[i].x;
            }
            if (array[i].y > max.y )
            {
                max.y = array[i].y;
            }
            if (array[i].z > max.z )
            {
                max.z = array[i].z;
            }
        }
    }
}

