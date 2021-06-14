using System;
using UnityEngine;
using System.Collections.Generic;
using Object = System.Object;

public class TankScript : VehicleScript
{
    [SerializeField]
    private List<GameObject> _wheels;
    [SerializeField]
    private GameObject _nozzle;
    [SerializeField]
    private GameObject _wheelBelt;
    [SerializeField]
    private GameObject _firePosition;
    [SerializeField] 
    private VehicleType _vehicleType;

    private GameObject _backgroundImage;
    private Vector3 _startPoint,_endPoint;
    private float _bufferWidth;
    private bool _gamePaused;
    private float _freeFallTime = 0.0f;
    private bool _freeFall = true;
    private Vector3 _freeFallStartPoint,_freeFallTargetPoint;
    private GameObject _root;
    
    public float VehicleWidth { get; private set; }
    
    void Start()
    {
        _2PI_R = Mathf.PI * _wheels[0].transform.localScale.x;
        _direction = _previousDirection = TranslationDirection.IDLE;
        _bulletEmitter = new BulletEmitter();
        VehicleWidth = _wheelBelt.transform.localScale.x;    
        
        TerrainLayoutController terrainsLayoutController = FindObjectOfType<TerrainLayoutController>();
        _backgroundImage = GameObject.FindWithTag("Background");

        _startPoint = terrainsLayoutController.GetStartPoint();
        _endPoint = terrainsLayoutController.GetEndPoint();

        _bufferWidth = _backgroundImage.transform.localScale.x;
       
        GameEventManager.Instance.SubscribeEventListener(Event.LeftArrowPressed , OnArrowPressed);
        GameEventManager.Instance.SubscribeEventListener(Event.RightArrowPressed , OnArrowPressed);
        GameEventManager.Instance.SubscribeEventListener(Event.ArrowReleased , OnArrowReleased);
        GameEventManager.Instance.SubscribeEventListener(Event.RightTouchUpdated , OnSliderUpdated);
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
 
        _firingTime = 0.0f;
        _emitterTime = 0.0f;
        _currentEmissionType = EmissionType.SINGLE;
        _numberOfBullets = 1;
        
        UpdateNozzle(90);//Initial nozzle direction
        _root = GameObject.FindGameObjectWithTag("GameEnvironmentRoot");
        
        RaycastHit2D info;
        Vector3 raycastPosition = gameObject.transform.position;
        info = Physics2D.Raycast(raycastPosition, -1.0f * Vector3.up,1000 , LayerMask.GetMask("Terrain"));

        if (info.collider != null)
        {
            _freeFallStartPoint = gameObject.transform.position;
            _freeFallTargetPoint = new Vector3(info.point.x,info.point.y,gameObject.transform.position.z);
        }
    }

    public void InitWithData(VehicleData data)
    {
        Data = data;
        if (Data != null)
        {
            _speed = Data.Speed; 
            _firingData = Data.GetFiringData(ArmorType.Bullet);
        }
    }
    
    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.LeftArrowPressed , OnArrowPressed);
        GameEventManager.Instance.UnsubscribeEventListener(Event.RightArrowPressed , OnArrowPressed);
        GameEventManager.Instance.UnsubscribeEventListener(Event.ArrowReleased , OnArrowReleased);
        GameEventManager.Instance.UnsubscribeEventListener(Event.RightTouchUpdated , OnSliderUpdated);
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
    }
    
    private void OnPausePressed(Object data)
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

        if (_freeFall)
        {
            DoFreeFall();
            return;
        }

        UpdateLinearMotion();
        AdjustHeightAndPitch(_direction);
        
        _startPoint = _backgroundImage.transform.localPosition;
        _startPoint.x -= _bufferWidth*0.5f - VehicleWidth ;        
        _previousDirection = _direction;
    }

    private void FixedUpdate()
    {
        if (_gamePaused)
        {
            return;
        }
        
        if (Data?.FiringData != null)
        {
            UpdateFiringLogic();   
        }
        _bulletEmitter?.Update();
    }

    private void DoFreeFall()
    {
        _freeFallTime += Time.deltaTime;

        if (_freeFallTime >= 1.0f)
        {
            _freeFallTime = 1.0f;
        }
        Vector3 position = Vector3.Lerp(_freeFallStartPoint,_freeFallTargetPoint, 1.0f-Mathf.Cos(_freeFallTime*Mathf.PI/2.0f));
        
        gameObject.transform.position = position;
        
       
        if (_freeFallTime == 1.0f)
        {
             _freeFall = false;
            GameEventManager.Instance.TriggerEvent(Event.OnPlayerReady , string.Empty);
        }
    }

    private void UpdateLinearMotion()
    {
        Vector3 position = gameObject.transform.position;
        float delta = _speed * Time.deltaTime;
        float angle = 2 * Mathf.PI * (delta / _2PI_R);

        if(_direction == TranslationDirection.LEFT)
        {
            position.x -= delta;
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            position.x += delta;
        }

        if( _direction != TranslationDirection.IDLE &&
            position.x > _startPoint.x && position.x < _endPoint.x )
        {
            gameObject.transform.position = position;
            
            if(_direction == TranslationDirection.LEFT)
            {
                if (position.x > _startPoint.x)
                {
                    GameEventManager.Instance.TriggerEvent(Event.OnPlayerMoveLeft , delta.ToString());   
                }
            }
            else if( _direction == TranslationDirection.RIGHT)
            {
                if (position.x < _endPoint.x - _bufferWidth * 0.4f)
                {
                    if (_previousDirection != TranslationDirection.RIGHT && _direction == TranslationDirection.RIGHT)
                    {
                        GameEventManager.Instance.TriggerEvent(Event.OnPlayerStartMovingRight , delta.ToString());
                    }
                    else
                    {
                        GameEventManager.Instance.TriggerEvent(Event.OnPlayerMoveRight , delta.ToString());
                    }
                       
                }
            }
            
            for(int i = 0; i < _wheels.Count; i++)
            {
                Vector3 eulerAngles = _wheels[i].transform.localRotation.eulerAngles;
                if( _direction == TranslationDirection.LEFT)
                {
                    eulerAngles.z += Mathf.Rad2Deg * angle;
                }
                else if( _direction == TranslationDirection.RIGHT)
                {
                    eulerAngles.z -= Mathf.Rad2Deg * angle;
                }
                _wheels[i].transform.localRotation = Quaternion.Euler(eulerAngles);
            }
        }

    }
    
    private void Fire()
    {
        CurrentFiringData currentFiringData = new CurrentFiringData();
        currentFiringData.emissionType = _currentEmissionType;
        currentFiringData.armorType = _firingData.Type;
        currentFiringData.count = _numberOfBullets;
        currentFiringData.startPosition = _firePosition.transform.position;
        currentFiringData.direction = _nozzle.transform.right;
        currentFiringData.damageUponImpact = _firingData.BaseDamageUponImpact;
        currentFiringData.source = Data.Type;
        
        _bulletEmitter.Fire(currentFiringData);
    }

    private void UpdateNozzle( float angleInDegree)
    {
        Vector3 eulerAngle = _nozzle.transform.localRotation.eulerAngles;
        eulerAngle.z = angleInDegree;
        _nozzle.transform.localRotation = Quaternion.Euler(eulerAngle);
    }
    
    private void OnArrowPressed(Object data)
    {
        Enum.TryParse(data.ToString(), out _direction);
    }

    private void OnArrowReleased(Object data)
    {
        Enum.TryParse(data.ToString(), out _direction);
        _previousDirection = _direction;
    }
    
    private void OnSliderUpdated(Object data)
    {
        Vector3 worldPosition = Utils.Vector3FromString(data.ToString());
        Vector3 direction = worldPosition - _nozzle.transform.position;
        direction.z = 0.0f;
        direction.Normalize();

        float signedAngle = Vector3.SignedAngle(gameObject.transform.right, direction , Vector3.forward);
        if (signedAngle >= -20 && signedAngle <= 180)
        {
            UpdateNozzle(signedAngle);   
        }
    }

    private void UpdateFiringLogic()
    {
        _emitterTime += Time.deltaTime;
        
        //Randomly choose emission type in every 2 second
        if (_emitterTime > 2.0f)
        {
            _emitterTime = 0.0f;
            _currentEmissionType = (EmissionType) UnityEngine.Random.Range(0, 3);
            
            if (_currentEmissionType == EmissionType.SINGLE)
            {
                _numberOfBullets = _firingData.NumberOfBulletsForLinearFire;
            }
            else if(_currentEmissionType == EmissionType.MULTIPLE)
            {
                _numberOfBullets = _firingData.NumberOfBulletsForMultipleFire;
            }
            else if( _currentEmissionType == EmissionType.MULTIPLE_RADIAL)
            {
                _numberOfBullets = _firingData.NumberOfBulletsForRadialFire;
            }
        }

        if (_currentEmissionType == EmissionType.MULTIPLE_RADIAL)
        {
            _firingTime +=_firingData.RadialFiringRate * Time.deltaTime;
        }
        else
        {
            _firingTime += _firingData.LinearFiringRate * Time.deltaTime;
        }

        if (_firingTime >= 1.0f)
        {
            Fire();
            _firingTime = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ammunition ammunition = other.gameObject.GetComponent<Ammunition>();
        if ( ammunition != null)
        {
            if (ammunition.Type == ArmorType.Rocket)
            {
                GameEventManager.Instance.TriggerEvent(Event.OnHitByRocket,ammunition.DamageOnHit.ToString()); 
                
                Vector3 hitPosition = other.gameObject.transform.position;
                hitPosition.z = 0.0f;
                InstanceData emitter = ParticlePool.Instance.GetObject(ParticleDataManager.Instance.GetPath(ParticleType.Explosion));
                emitter.Instance.transform.parent = _root.transform;
                emitter.Instance.transform.position = hitPosition;
            }
            else if(ammunition.Type == ArmorType.Bullet)
            {
                GameEventManager.Instance.TriggerEvent(Event.OnHitByBullet,ammunition.DamageOnHit.ToString());
                
                Vector3 hitPosition = other.gameObject.transform.position;
                hitPosition.z = 0.0f;
                InstanceData emitter = ParticlePool.Instance.GetObject(ParticleDataManager.Instance.GetPath(ParticleType.Spark));
                emitter.Instance.transform.parent = _root.transform;
                emitter.Instance.transform.position = hitPosition;
            }
        }

        if (other.gameObject.CompareTag("Star"))
        {
            other.gameObject.SetActive(false);
            GameEventManager.Instance.TriggerEvent(Event.OnStarCollected , string.Empty);
        }
        
        if (other.gameObject.CompareTag("AI"))
        {
            Destroy(other.gameObject);
            
            GameEventManager.Instance.TriggerEvent(Event.OnHItByAI , "50");
            GameEventManager.Instance.TriggerEvent(Event.OnAIGotKilled , other.gameObject.GetComponent<VehicleScript>());
            
            Vector3 hitPosition = other.gameObject.transform.position;
            hitPosition.z = 0.0f;
            InstanceData emitter = ParticlePool.Instance.GetObject("CartoonFX/CFXPrefabs/Explosions/CFX_Explosion");
            emitter.Instance.transform.parent = _root.transform;
            emitter.Instance.transform.position = hitPosition;
        }

        if (other.gameObject.CompareTag("Flag"))
        {
            GameEventManager.Instance.TriggerEvent(Event.OnFlagCollected , string.Empty);
        }
    }

    private void AdjustHeightAndPitch(TranslationDirection movementDirection)
    {
        RaycastHit2D info1,info2,info3;
        Vector3 midPosition = gameObject.transform.position;
        midPosition.y += 200.0f;
        
        Vector3 frontPosition = midPosition;
        frontPosition.x += VehicleWidth;
        
        Vector3 backPosition = midPosition;
        backPosition.x -= VehicleWidth;
        
        info1 = Physics2D.Raycast(midPosition, -1.0f * Vector3.up,300 , LayerMask.GetMask("Terrain"));
        info2 = Physics2D.Raycast(frontPosition, -1.0f * Vector3.up,300 , LayerMask.GetMask("Terrain"));
        info3 = Physics2D.Raycast(backPosition, -1.0f * Vector3.up,300 , LayerMask.GetMask("Terrain"));

        if (info1.collider != null)
        {
            midPosition.y = info1.point.y;
            gameObject.transform.position = midPosition;
        }
        
        if (info2.collider != null && info3.collider != null )
        {
            Vector2 direction2D = info2.point - info3.point;
            direction2D.Normalize();
            Vector3 direction = new Vector3(direction2D.x,direction2D.y,0.0f);
            float angle = Mathf.Acos(Vector3.Dot(direction, Vector3.right))* Mathf.Rad2Deg;
            Vector3 normalVector = Vector3.Cross(direction,Vector3.right);
            if (normalVector.z > 0)
            {
                angle *= -1.0f;
            }
            gameObject.transform.rotation = Quaternion.Euler(0,0,angle);
        }
    }
}

public partial class Event
{
    public static string OnPlayerMoveLeft = "OnPlayerMoveLeft";
    public static string OnPlayerMoveRight = "OnPlayerMoveRight";
    public static string OnPlayerReady = "OnPlayerReady";
    public static string OnPlayerStartMovingRight = "OnPlayerStartMovingRight";
    public static string OnHitByRocket = "HitByRocket";
    public static string OnHitByBullet = "HitByBullet";
    public static string OnHItByAI = "OnHItByAI";
    public static string OnStarCollected = "OnStarCollected";
    public static string OnFlagCollected = "OnFlagCollected";
}


