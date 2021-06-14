using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class GameStats : Singleton<GameStats>
{
    public float GameTime { get; private set; }
    public uint Score { get; set; }
    public uint Multiplier { get; set; }
    public uint StarsCollected { get; set; }
    public Dictionary<VehicleType,int> BotsKilled { get; set; }
    public Dictionary<VehicleType,int> BotsSpawned { get; set; }
    public bool PlayerDied { get; private set; }
    public bool PlayerReachedTarget { get; private set; }
    
    private float _scoreTimer;
    private bool _pause;

    public GameStats()
    {
        Reset();
        BotsKilled = new Dictionary<VehicleType, int>();
        BotsSpawned = new Dictionary<VehicleType, int>();
    }

    public void AddListeners()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.OnAISpawned ,OnAISpawned );
        GameEventManager.Instance.SubscribeEventListener(Event.OnAIGotKilled , OnAIGotKilled );
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPause);
        GameEventManager.Instance.SubscribeEventListener(Event.OnStarCollected ,OnStarCollected );
        GameEventManager.Instance.SubscribeEventListener(Event.OnMissionStarted ,OnMissionStarted );
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerDead , OnPlayerDead );
        GameEventManager.Instance.SubscribeEventListener(Event.OnFlagCollected , OnFlagCollected);
    }

    public void RemoveListeners()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnAISpawned ,OnAISpawned );
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnAIGotKilled , OnAIGotKilled );
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPause);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnStarCollected ,OnStarCollected );
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnMissionStarted ,OnMissionStarted );
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerDead , OnPlayerDead );
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnFlagCollected , OnFlagCollected);
    }
    
    public void Reset()
    {
        GameTime = 0.0f;
        Multiplier = 1;
        Score = 0;
        StarsCollected = 0;
        _scoreTimer = 0.0f;
        _pause = false;
        PlayerDied = false;
        PlayerReachedTarget = false;
    }
    
    public void Update()
    {
        if (_pause)
        {
            return;
        }
        
        GamePlayState state = AppStateMachine.Instance.CurrentState as GamePlayState;
        if (state == null)
        {
            return;
        }
        
        GameTime += Time.deltaTime;
        _scoreTimer += Time.deltaTime;
        if (_scoreTimer > 1.0f)
        {
            if (state.CurrentPlayer.CurrentHealth > 0)
            {
                Score += Multiplier * 10;   
            }
            _scoreTimer = 0.0f;
        }
        Multiplier = (uint) (GameTime / 10.0f) + 1;
    }

    void OnMissionStarted(Object data)
    {
        Reset();
    }
    
    void OnPause(Object data)
    {
        _pause = Boolean.Parse(data.ToString());
    }

    void OnRestart(Object data)
    {
        GameTime = _scoreTimer = 0.0f;
        Multiplier = 1;
    }

    void OnAISpawned(Object data)
    {
        VehicleScript vehicle = data as VehicleScript;
        if (vehicle != null)
        {
            VehicleType vehicleType = vehicle.Data.Type;
            if (BotsSpawned.ContainsKey(vehicleType))
            {
                BotsSpawned[vehicleType]++;
            }
            else
            {
                BotsSpawned.Add(vehicleType,1);
            }
        }
    }
    
    void OnAIGotKilled(Object data)
    {
        VehicleScript vehicle = data as VehicleScript;
        if (vehicle != null)
        {
            Score += (uint)vehicle.Data.MaxHealth;
            VehicleType vehicleType = vehicle.Data.Type;
            if (BotsKilled.ContainsKey(vehicleType))
            {
                BotsKilled[vehicleType]++;
            }
            else
            {
                BotsKilled.Add(vehicleType,1);
            }
        }
    }

    void OnStarCollected(Object data)
    {
        StarsCollected++;
    }

    void OnPlayerDead(Object data)
    {
        PlayerDied = true;
    }

    void OnFlagCollected(Object data)
    {
        PlayerReachedTarget = true;
    }
}
