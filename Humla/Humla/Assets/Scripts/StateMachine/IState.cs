
public enum AppStateType
{
    Menu,
    GamePlay,
    GameEnd,
}

public interface IState
{
    void OnEnter(IState previousState);
    void OnExit();
    void Update();
    
    AppStateType StateType { get; set; }
}
