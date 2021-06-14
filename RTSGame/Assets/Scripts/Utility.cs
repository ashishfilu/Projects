using UnityEngine;

public class Utility 
{
    public static Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float entry;
        if (plane.Raycast(ray, out entry))
        {
            return ray.GetPoint(entry);
        }
        return Vector3.positiveInfinity;
    }
}
