using UnityEngine;

public class Ammunition : MonoBehaviour
{
    public ArmorType Type;
    public float DamageOnHit { get; set; }
    public VehicleType Source { get; set; }
    protected bool _gamePaused;
}
