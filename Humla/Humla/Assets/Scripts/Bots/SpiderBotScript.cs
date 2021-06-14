using System;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public enum AxisToClamp
{
    XAxis=0,
    YAxis=1,
}

public enum LandedArea
{
    MidAirRamp,
    Terrain,
}

public class SpiderBotScript : VehicleScript
{
    public GameObject _botObject;
    public GameObject _nozzle;
    public GameObject _firingPosition;
    
    private GameObject _root;
    private RaycastHit2D _previousRaycastHitInfo;
    private AxisToClamp _axisToClamp;
    private LandedArea _landedArea;
    
    private float _freeFallTime = 0.0f;
    private Vector3 _freeFallStartPoint,_freeFallTargetPoint;
    
    private string _layerMask;
    private bool _gamePaused;
    private AnimationController _animationController;
    private float _health;
    private BotStateMachine _stateMachine;
    private State _currentState;
    private TankScript _playerVehicle = null;
    
    private Vector3 _startDirection, _targetDirection;
    private float _rotationTime ;

    void Start()
    {
        _root = GameObject.FindGameObjectWithTag("GameEnvironmentRoot");
        
        _direction = (TranslationDirection) Random.Range(0, 2);
        _axisToClamp = AxisToClamp.YAxis;
        _animationController = _botObject.GetComponent<AnimationController>();
        _nozzle.SetActive(false);
        
        RaycastHit2D info;
        Vector3 raycastPosition = gameObject.transform.position;
        Vector3 raycastDirection = -1.0f * Vector3.up;
        info = Physics2D.Raycast(raycastPosition, raycastDirection,2000 , LayerMask.GetMask(new []{"MidAirRamp", "Terrain"}));
        
        _freeFallStartPoint = gameObject.transform.position;
        _freeFallTargetPoint = new Vector3(info.point.x,info.point.y,gameObject.transform.position.z);
        
        _layerMask = LayerMask.LayerToName(info.transform.gameObject.layer);
        _landedArea = LandedArea.MidAirRamp;
        _previousRaycastHitInfo = info;
        
        if (_layerMask == "Terrain")
        {
            _direction = TranslationDirection.LEFT;
            _landedArea = LandedArea.Terrain;
        }

        _stateMachine = new BotStateMachine();
        _stateMachine.AddState(StateType.Freefall);
        _stateMachine.AddState(StateType.Idle, _landedArea == LandedArea.MidAirRamp ? 0.3f:0.1f);
        _stateMachine.AddState(StateType.ActivateNozzle,_landedArea == LandedArea.MidAirRamp? 1.0f:0.2f);
        _stateMachine.AddState(StateType.Walk, _landedArea == LandedArea.MidAirRamp? 4.0f:1.0f);
        _stateMachine.AddState(StateType.Idle,_landedArea == LandedArea.MidAirRamp?1.0f:0.1f);
        _stateMachine.AddState(StateType.Aim,1.0f);
        _stateMachine.AddState(StateType.Shoot,2.0f);

        _stateMachine.OnStateChanged += OnStateChanged;
        _stateMachine.OnStateExit += OnStateExit;

        _currentState = null;

        InitWithData();
        
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
        
        GameEventManager.Instance.TriggerEvent(Event.OnAISpawned,this);
    }

    private void OnDestroy()
    {
        _stateMachine.OnStateChanged -= OnStateChanged;
        _stateMachine.OnStateExit -= OnStateExit;
        _stateMachine = null;
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
    }

    public void InitWithData()
    {
        Data = VehicleController.Instance.GetVehicle("SpiderBot_1").Data;  
        if (Data != null)
        {
            _health = Data.MaxHealth;
            _speed = Data.Speed;
            _firingData = Data.GetFiringData(ArmorType.Bullet);
        }
        _bulletEmitter = new BulletEmitter();
    }
    
    void Update()
    { 
        GamePlayState state = AppStateMachine.Instance.CurrentState as GamePlayState;
        
        if (_playerVehicle == null && state != null)
        {
            Player player = state.CurrentPlayer;
            if (player != null)
            {
                _playerVehicle = state.CurrentPlayer.Vehicle;   
            }
        }
        
        if ((!IsVisible() && _landedArea == LandedArea.MidAirRamp)|| _gamePaused )
        {
            return;
        }

        if (_health < 0)
        {
            gameObject.SetActive(false);
            GameEventManager.Instance.TriggerEvent(Event.OnAIGotKilled,this);
            return;
        }
        
        _stateMachine.Update();
        
        switch (_currentState.Type)
        {
            case StateType.Freefall:
                DoFreeFall();
                break;
            case StateType.ActivateNozzle:
                _nozzle.SetActive(true);
                break;
            case StateType.Walk:
                Walk(); 
                break;
            case StateType.Idle:
                break;
            case StateType.Aim:
                DoAim();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!IsVisible() || _gamePaused || _currentState == null)
        {
            return;
        }
        
        switch (_currentState.Type)
        {
            case StateType.Shoot:
                UpdateFiringLogic();
                break;
        }
        _bulletEmitter.Update();
    }

    private void OnStateChanged(State currentState)
    {
        _currentState = currentState;

        switch (currentState.Type)
        {
            case StateType.Aim:
                _startDirection = _nozzle.transform.up;
                _targetDirection = (_playerVehicle.transform.position - gameObject.transform.position).normalized;
                _rotationTime = 0.0f;
                break; 
            case StateType.Walk:
                _animationController.SetBoolVariableState("Walk",true);
                break;
        }
    }

    private void OnStateExit(State exitState)
    {
        switch (exitState.Type)
        {
            case StateType.Walk:
                _animationController.SetBoolVariableState("Walk",false);
                break;
            case StateType.Shoot:
                _direction = (TranslationDirection)Random.Range(0, 2);
                _stateMachine.AddState(StateType.Walk,Random.Range(1.0f,1.5f));
                _stateMachine.AddState(StateType.Aim,Random.Range(0.5f,0.7f));
                _stateMachine.AddState(StateType.Shoot,Random.Range(1.5f,2.0f));
                break;
                
        }
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
        
        if (_freeFallTime > 0.8f && _animationController.GetBoolVariableState("Landing") == false)
        {
            _animationController.SetBoolVariableState("Landing",true);
        }
       
        if (_freeFallTime == 1.0f)
        {
            _stateMachine.ForceChangeState();
        }
    }

    private void Walk()
    {
        Vector3 position = gameObject.transform.position;
        if (_direction == TranslationDirection.LEFT)
        {
            position -= gameObject.transform.right * Time.deltaTime * _speed;   
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            position += gameObject.transform.right * Time.deltaTime * _speed;
        }

        if (_landedArea == LandedArea.MidAirRamp)
        {
            WalkOnRamp(position);
        }
        else if (_landedArea == LandedArea.Terrain)
        {
            WalkOnTerrain(position);
        }
    }

    private void WalkOnRamp(Vector3 position)
    {
        RaycastHit2D info;
        Vector3 raycastDirection = (position - gameObject.transform.position).normalized;
        info = Physics2D.Raycast(position, raycastDirection,10 , LayerMask.GetMask(_layerMask));
    
        if (info.collider != null)
        {
            _previousRaycastHitInfo = info;
            position.x = info.point.x;
            position.y = info.point.y;
        }
        else
        {
            if (_direction == TranslationDirection.LEFT)
            {
                Vector3 eulerAngles = gameObject.transform.localEulerAngles;
                eulerAngles.z += 90 * Time.deltaTime;
                gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
    
                int axis = (int) _axisToClamp + 1;
                if (axis > 1)
                {
                    axis = 0;
                }
    
                _axisToClamp = (AxisToClamp) axis;
                
                if (_axisToClamp == AxisToClamp.YAxis)
                {
                    position.y = _previousRaycastHitInfo.point.y;
                }
                else if( _axisToClamp == AxisToClamp.XAxis)
                {
                    position.x = _previousRaycastHitInfo.point.x;
                }
            }
            else if (_direction == TranslationDirection.RIGHT)
            {
                Vector3 eulerAngles = gameObject.transform.localEulerAngles;
                eulerAngles.z -= 90 * Time.deltaTime;
                gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
    
                int axis = (int) _axisToClamp + 1;
                if (axis > 1)
                {
                    axis = 0;
                }
    
                _axisToClamp = (AxisToClamp) axis;
                
                if (_axisToClamp == AxisToClamp.YAxis)
                {
                    position.y = _previousRaycastHitInfo.point.y;
                }
                else if( _axisToClamp == AxisToClamp.XAxis)
                {
                    position.x = _previousRaycastHitInfo.point.x;
                }
            }
        }
        
        gameObject.transform.position = position;
    }

    private void WalkOnTerrain(Vector3 position)
    {
        float distanceFromPlayer = (position - _playerVehicle.transform.position).magnitude;
        if (distanceFromPlayer < 200.0f)
        {
            _animationController.PauseCurrentAnimation();
            _stateMachine.ForceChangeState();
        }
        else
        {
            _animationController.UnpauseCurrentAnimation();
            gameObject.transform.position = position;
        }

        AdjustHeightAndPitch();
    }
    
    private void AdjustHeightAndPitch()
    {
        RaycastHit2D info1,info2,info3;
        Vector3 midPosition = gameObject.transform.position;
        midPosition.y += 200.0f;
        
        Vector3 frontPosition = midPosition;
        frontPosition.x += 50.0f;
        
        Vector3 backPosition = midPosition;
        backPosition.x -= 50.0f;
        
        //Slow slow . Need to find better way
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
            float angle = Mathf.Acos(Vector3.Dot(direction, Vector3.right)) * Mathf.Rad2Deg;
            Vector3 normalVector = Vector3.Cross(direction,Vector3.right);
            if (normalVector.z > 0)
            {
                angle *= -1.0f;
            }
            gameObject.transform.rotation = Quaternion.Euler(0,0,angle);
        }
    }
    
    private void DoAim()
    {
        _rotationTime += Time.deltaTime * 3.0f;
        Vector3 direction = Vector3.Lerp(_startDirection,_targetDirection,_rotationTime);
        _nozzle.transform.up = direction;
    }
    
    private void UpdateFiringLogic()
    {
        _firingTime += _firingData.LinearFiringRate * Time.deltaTime;
        if (_firingTime >= 1.0f)
        {
            Fire();
            _firingTime = 0.0f;
            _nozzle.transform.up = (_playerVehicle.transform.position - gameObject.transform.position).normalized;
        }
    }

    private void Fire()
    {
        CurrentFiringData currentFiringData = new CurrentFiringData();
        currentFiringData.emissionType = EmissionType.SINGLE;
        currentFiringData.armorType = _firingData.Type;
        currentFiringData.count = _numberOfBullets;
        currentFiringData.startPosition = _firingPosition.transform.position;
        currentFiringData.direction = _nozzle.transform.up;
        currentFiringData.damageUponImpact = _firingData.BaseDamageUponImpact;
        currentFiringData.source = Data.Type;
        
        _bulletEmitter.Fire(currentFiringData);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 hitPosition;
        Ammunition ammo = other.gameObject.GetComponent<Ammunition>();

        if (ammo != null && ammo.Source == VehicleType.Tank)
        {
            hitPosition = other.gameObject.transform.position;
            hitPosition.z = 0.0f;
            InstanceData emitter = ParticlePool.Instance.GetObject(ParticleDataManager.Instance.GetPath(ParticleType.Spark));
            emitter.Instance.transform.parent = _root.transform;
            emitter.Instance.transform.position = hitPosition;
            _health -= ammo.DamageOnHit;
        }
    }
    
    private bool IsVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider2D>().bounds))
        {
            return true;   
        }
        return false;
    }
    
    private void OnPausePressed(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
        _animationController.PauseAllAnimation(_gamePaused);
    }
}
