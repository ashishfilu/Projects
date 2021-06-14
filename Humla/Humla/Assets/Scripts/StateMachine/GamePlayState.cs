using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;


public class GamePlayState : IState
{
    public Player CurrentPlayer { get; private set; }
    public float GameDuration { get; private set; }
    
    private bool _gamePaused;
    private PopUp _popUpScript;
    private AppStateType _type;
    private bool _updateTimer;
    private int _rewardCoins;
    
    public AppStateType StateType
    {
        get { return _type; }
        set { _type = value; }
    }
    
    public GamePlayState()
    {
        _type = AppStateType.GamePlay;
    }
    
    public void OnEnter(IState previousState)
    {
        MissionData missionData = MissionController.Instance.Mission.Data;
        CurrentPlayer = null;
        GameDuration = missionData.Duration;
        
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnPausePressed);
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerReady , OnPlayerReady);
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerDead , OnPlayerDied);
        GameEventManager.Instance.SubscribeEventListener(Event.OnFlagCollected , OnFlagCollected);

        if (VehicleController.Instance.SelectedVehicle == null)
        {
            User.Instance.SetDefaultSelectedVehicle();
        }
        
        GameStats.Instance.AddListeners();
        
        SceneLoader.Instance.AddRequest(missionData.Path, true, null, OnSceneLoaded);
        
        _updateTimer = false;
        _gamePaused = false;
    }

    public void Update()
    {
        if (_gamePaused)
        {
            return;
        }

        if (CurrentPlayer != null)
        {
            ObjectPool.Instance.Update();
            ParticlePool.Instance.Update();
            GameStats.Instance.Update();
        }

        if (_updateTimer)
        {
            GameDuration -= Time.deltaTime;   
        }

        if (GameDuration <= 0)
        {
            GameEventManager.Instance.TriggerEvent(Event.PausePressed,"true");
            AppStateMachine.Instance.SetCurrentState(AppStateType.GameEnd);
        }
    }

    public void OnExit()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnPausePressed);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerReady , OnPlayerReady);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerDead , OnPlayerDied);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnFlagCollected , OnFlagCollected);
        
        CurrentPlayer.Destroy();
        ObjectPool.Instance.ClearPool();
        ParticlePool.Instance.ClearPool();
        PopUpManager.Instance.Clear();
        
        _updateTimer = false;
        _gamePaused = false;
    }
    
    private void OnPausePressed(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
    }

    private void OnPlayerReady(Object data)
    {
        _updateTimer = true;
    }
    
    private void OnPlayerDied(Object data)
    {
        AppStateMachine.Instance.SetCurrentState(AppStateType.GameEnd);
    }
    
    private void OnSceneLoaded(Scene scene)
    {
        MainInstance.Instance.StartCoroutine(InitializePlayer());
    }

    private void OnFlagCollected(Object data)
    {
        GameEventManager.Instance.TriggerEvent(Event.PausePressed,"true");
        AppStateMachine.Instance.SetCurrentState(AppStateType.GameEnd);
    }
    
    private IEnumerator InitializePlayer()
    {
        yield return new WaitForSeconds(0.1f);
        CurrentPlayer = new Player();
        CurrentPlayer.Initialize(VehicleController.Instance.SelectedVehicle);
        
        GameEventManager.Instance.TriggerEvent(Event.OnMissionStarted , string.Empty);
    }
}