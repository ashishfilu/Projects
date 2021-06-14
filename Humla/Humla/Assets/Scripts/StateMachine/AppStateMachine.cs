
using System.Collections.Generic;
using UnityEngine;

public class AppStateMachine : Singleton<AppStateMachine>
{
    private Dictionary<AppStateType,IState> _allStates;
    public IState CurrentState { get; private set; }

    public AppStateMachine()
    {
        _allStates = new Dictionary<AppStateType, IState>();
        CurrentState = null;
    }

    public void InitializeGameStates()
    {
        _allStates.Add(AppStateType.Menu,new MenuState());
        _allStates.Add(AppStateType.GamePlay,new GamePlayState());
        _allStates.Add(AppStateType.GameEnd , new GameEndState());
    }

    public void SetCurrentState(AppStateType type)
    {
        IState previousState = CurrentState;
        previousState?.OnExit();
        CurrentState = _allStates[type];
        Debug.Assert(CurrentState != null);
        CurrentState.OnEnter(previousState);
        GameEventManager.Instance.TriggerEvent(Event.OnStateChanged,CurrentState);
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}

public partial class Event
{
    public static string OnStateChanged = "OnStateChanged";
}
