using UnityEngine; 

public class Shape : MonoBehaviour
{
    public enum Type
    {
        Cube=0,
        Sphere,
    };
    
    public enum Operation
    {
        Intersect=0,
        Union,
        Difference,
        Blend,
        None,
    }

    public Type _shapeType;
    public Operation _operation;
    public Color _color = Color.white;

    [Range(0,1)]
    public float _blendStrength;

    public Vector3 Position => transform.position;
    public Vector3 Scale => transform.localScale;
}

