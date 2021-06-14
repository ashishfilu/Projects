using System;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Spaceship : HelicopterScript
{
    [SerializeField] private GameObject _nozzleAnchorPoint;
    [SerializeField] private GameObject _nozzle;

    private BotStateMachine _stateMachine;
    private AnimationController _animationController;
    private TankScript _playerVehicle = null;
    private State _currentState;
    private float _firingTimer;
    private float _flyingSpeed = 20.0f;
        
    void Start()
    {
        Initialize();
        _animationController = gameObject.GetComponentInChildren<AnimationController>();
        InitWithData();
        
        GameEventManager.Instance.SubscribeEventListener(Event.OnMissionStarted , OnMissionStrated);
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    protected override void InitWithData()
    {
        Data = VehicleController.Instance.GetVehicle("Spaceship_1").Data;
        if (Data != null)
        {
            _firingData = Data.GetFiringData(ArmorType.Bullet);
        }
        _stateMachine = new BotStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
        _stateMachine.OnStateExit += OnStateExit;
        
        _stateMachine.AddState(StateType.FlyIn,2.0f);
        _stateMachine.AddState(StateType.Shoot,5.0f);
        _stateMachine.AddState(StateType.FlyOut);
        
        Reset();
    }
    
    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnGamePaused);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnMissionStarted , OnMissionStrated);
    }

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
            ResetStateMachine();
            _stateMachine.ForceChangeState();
            return;
        }
        
        _stateMachine.Update();
        _bulletEmitter.Update();
        UpdateStartAndEndPoint();
        
        if (_currentState == null)
        {
            return;
        }
        
        switch (_currentState.Type)
        {
            case StateType.FlyIn:
                DoFlyInAnimation();
                break;
            case StateType.FlyOut:
                DoFlyOutAnimation();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_gamePaused || _health <= 0 || _currentState == null )
        {
            return;
        }
        
        switch (_currentState.Type)
        {
            case StateType.Shoot:
                DoAimAndShoot();
                break;
        }
    }

    protected override void Reset()
    {
        base.Reset();
        _currentState = null;
    }

    private void DoFlyInAnimation()
    {
        Vector3 position = gameObject.transform.localPosition;
        float ratio = _currentState.CurrentTime / _currentState.Duration;
        
        float speed = Mathf.Lerp(_speed, _flyingSpeed, ratio);
        
        if (_direction == TranslationDirection.LEFT)
        {
            position -= Vector3.right * Time.deltaTime * speed;
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            position +=  Vector3.right * Time.deltaTime * speed;
        }

        gameObject.transform.localPosition = position;
    }
    
    private void DoFlyOutAnimation()
    {
        Vector3 position = gameObject.transform.localPosition;
        float speed = Mathf.Lerp(_flyingSpeed, _speed, _currentState.CurrentTime/2.0f);
        
        if (_direction == TranslationDirection.LEFT)
        {
            position -= Vector3.right * Time.deltaTime * speed;
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            position +=  Vector3.right * Time.deltaTime * speed;
        }

        gameObject.transform.localPosition = position;
        
        if(_direction == TranslationDirection.LEFT)
        {
            if (position.x < _endPoint.x)
            {
                Reset();
                ResetStateMachine();
                _stateMachine.ForceChangeState();
            }
        }
        else if( _direction == TranslationDirection.RIGHT)
        {
            if (position.x > _endPoint.x)
            {
                Reset();
                ResetStateMachine();
                _stateMachine.ForceChangeState();
            }
        }
    }
    
    private void DoAimAndShoot()
    {
        Vector3 nozzlePosition = _nozzleAnchorPoint.transform.position;
        Vector3 playerPosition = _playerVehicle.transform.position;
        Vector3 direction = (playerPosition - nozzlePosition).normalized;
        _nozzleAnchorPoint.transform.right = direction;
        UpdateFiringLogic();
    }
    
    protected override void UpdateFiringLogic()
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

    protected override void Fire()
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
    
    private void OnStateChanged(State currentState)
    {
        _currentState = currentState;
    }

    private void OnStateExit(State exitState)
    {
        switch (exitState.Type)
        {
            case StateType.FlyIn:
                _animationController.SetBoolVariableState("Idle" , false);
                _animationController.SetBoolVariableState("Hover" , true);
                break;
            case StateType.Shoot:
                _animationController.SetBoolVariableState("Idle" , true);
                _animationController.SetBoolVariableState("Hover" , false);
                break;
        }
    }

    private void ResetStateMachine()
    { 
        _stateMachine.AddState(StateType.FlyIn, Random.Range(1.0f,1.5f));
        _stateMachine.AddState(StateType.Shoot,Random.Range(4.0f,6.0f));
        _stateMachine.AddState(StateType.FlyOut);

        _animationController.SetBoolVariableState("Idle" , true);
        _animationController.SetBoolVariableState("Hover" , false);
        
        _currentState = null;
    }

    private void OnMissionStrated(Object data)
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
    }

    private void OnGamePaused(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
        _animationController.PauseAllAnimation(_gamePaused);
    }
}
