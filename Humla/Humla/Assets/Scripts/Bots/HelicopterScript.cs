using System;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class HelicopterScript : VehicleScript
{
    [SerializeField] protected GameObject _firePosition;
    
    protected GameObject _backgroundImage;
    protected TankScript _tank;
    protected Vector3 _startPoint;
    protected Vector3 _endPoint;
    protected GameObject _root;
    protected float _health;
    protected bool _gamePaused;
    
    private float _firingTimer;
    private float _bgWidth, _bgHeight;
    
    void Start()
    {
        Initialize();
        InitWithData();
    }

    protected void Initialize()
    {
        _root = GameObject.FindGameObjectWithTag("GameEnvironmentRoot");
        _tank = GameObject.FindObjectOfType<TankScript>();
        _backgroundImage = GameObject.FindWithTag("Background");
        _bulletEmitter = new BulletEmitter();
        
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
        _health = 100;
        _speed = 200.0f;
    }

    protected virtual void InitWithData()
    {
        Data = VehicleController.Instance.GetVehicle("Helicopter_1").Data;
        if (Data != null)
        {
            _firingData = Data.GetFiringData(ArmorType.Rocket);
        }
        Reset();
    }
    
    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    protected void OnPausePressed(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (_gamePaused)
        {
            return;
        }
        if (_health <= 0)
        {
            GameEventManager.Instance.TriggerEvent(Event.OnAIGotKilled,this);
            Reset();
            return;
        }
        
        Vector3 position = gameObject.transform.localPosition;
        float delta = _speed * Time.deltaTime;

        if(_direction == TranslationDirection.LEFT)
        {
            position.x -= delta;

            if (position.x < _endPoint.x)
            {
                Reset();
                return;
            }
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            position.x += delta;
            
            if (position.x > _endPoint.x)
            {
                Reset();
                return;
            }
        }
        
        gameObject.transform.localPosition = position;
        UpdateStartAndEndPoint();

        UpdateFiringLogic();
        _bulletEmitter.Update();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_gamePaused)
        {
            return;
        }

        Vector3 hitPosition;
        Ammunition ammo = other.gameObject.GetComponent<Ammunition>();

        if (ammo != null)
        {
            hitPosition = other.gameObject.transform.position;
            hitPosition.z = 0.0f;
            InstanceData emitter = ParticlePool.Instance.GetObject(ParticleDataManager.Instance.GetPath(ParticleType.Spark));
            emitter.Instance.transform.parent = _root.transform;
            emitter.Instance.transform.position = hitPosition;
            _health -= ammo.DamageOnHit;
        }
    }
    protected virtual void Reset()
    {
        _direction = TranslationDirection.LEFT;//(TranslationDirection) Random.Range(0, 2);
        UpdateStartAndEndPoint();
        
        float height = _backgroundImage.transform.localScale.y;
        Quaternion rotation = Quaternion.Euler(0,0,0);
        if (_direction == TranslationDirection.LEFT)
        {
            rotation = Quaternion.Euler(0,180,0);
        }
        _startPoint.y = Random.Range(height * 0.15f, height * 0.25f);
        _endPoint.y = _startPoint.y;

        gameObject.transform.position = _startPoint;
        gameObject.transform.rotation = rotation;

        if (Data != null)
        {
            _speed = Data.Speed;
            _health = Data.MaxHealth;
        }

        _firingTimer = 0.9f;
        _numberOfBullets = 1;
        
        GameEventManager.Instance.TriggerEvent(Event.OnAISpawned,this);
    }

    protected void UpdateStartAndEndPoint()
    {
        //Update start and end point as player moves
        _startPoint = _endPoint = _backgroundImage.transform.position;
        float width = _backgroundImage.transform.localScale.x;
        if (_direction == TranslationDirection.LEFT)
        {
            _startPoint.x += width * 0.6f;
            _endPoint.x -= width * 0.6f ;
        }
        else if (_direction == TranslationDirection.RIGHT)
        {
            _startPoint.x -= width * 0.6f;
            _endPoint.x += width * 0.6f ;
        }
    }

    protected virtual void UpdateFiringLogic()
    {
        if (_firingData == null)
        {
            return;
        }
        _firingTimer += _firingData.LinearFiringRate * Time.deltaTime;
        if (_firingTimer >= 1.0f)
        {
            Fire();
            _firingTimer = 0.0f;
        }
    }

    protected virtual void Fire()
    {
        CurrentFiringData currentFiringData = new CurrentFiringData();
        currentFiringData.emissionType = _currentEmissionType;
        currentFiringData.armorType = _firingData.Type;
        currentFiringData.count = _numberOfBullets;
        currentFiringData.startPosition = _firePosition.transform.position;
        currentFiringData.direction = gameObject.transform.right;
        currentFiringData.damageUponImpact = _firingData.BaseDamageUponImpact;
        currentFiringData.source = Data.Type;
        
        _bulletEmitter.DropRocket(currentFiringData);
    }
}

public partial class Event
{
    public static string OnAIGotKilled = "OnAIGotKilled";
    public static string OnAISpawned = "OnAISpawned";
}
