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

        if (0.0f < u && u < 1.0f && t > 0.0f)
        {
            intersect = true;
            point.x = p1.x + t * (p2.x - p1.x);
            point.y = p1.y + t * (p2.y - p1.y);
            point.z = 0;
        }
    }
}
