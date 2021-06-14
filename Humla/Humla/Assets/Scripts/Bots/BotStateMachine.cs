using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,
    Jump,
    Walk,
    ActivateNozzle,
    Aim,
    Freefall,
    Shoot,
    FlyIn,
    FlyOut,
    None,
}

public class State
{
    public StateType Type;
    public float Duration;
    public float CurrentTime;

    public State(StateType type, float duration)
    {
        Type = type;
        Duration = duration;
        CurrentTime = 0.0f;
    }
}

public delegate void OnStateChangedDelegate(State currentState);

public delegate void OnStateExitDelegate(State exitState);

public class BotStateMachine
{
    private Queue<State> _allStates;
    private State _currentState = null;
    
    public event OnStateChangedDelegate OnStateChanged;
    public event OnStateExitDelegate OnStateExit;
    
    public BotStateMachine()
    {
        _allStates = new Queue<State>();
    }
    
    public void Update()
    {
        if (_currentState == null)
        {
            _currentState = _allStates.Dequeue();
            OnStateChanged?.Invoke(_currentState);
        }

        _currentState.CurrentTime += Time.deltaTime;

        if ( _currentState.Duration != -1 &&
             _currentState.CurrentTime >= _currentState.Duration)
        {
            ForceChangeState();
        }
    }

    public void ForceChangeState()
    {
        if (_currentState != null)
        {
            OnStateExit?.Invoke(_currentState);   
        }
        _currentState = null;
    }

    public void AddState(StateType type , float duration=-1.0f)
    {
        _allStates.Enqueue(new State(type,duration));
    }
}
