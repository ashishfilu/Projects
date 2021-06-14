using UnityEngine;

public enum TranslationDirection
{
    LEFT=0,
    RIGHT,
    IDLE,
}

public class VehicleScript : MonoBehaviour
{
    protected FiringData _firingData;
    protected TranslationDirection _direction,_previousDirection;
    protected BulletEmitter _bulletEmitter;
    protected EmissionType _currentEmissionType;
    protected float _firingTime,_emitterTime;
    protected int _numberOfBullets;
    protected float _speed = 100;
    protected float _2PI_R;
    
    public VehicleData Data { get; protected set; }
}
