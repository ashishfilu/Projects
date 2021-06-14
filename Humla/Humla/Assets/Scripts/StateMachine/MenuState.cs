
using UnityEngine.SceneManagement;

public class MenuState : IState
{
    private AppStateType _type;

    public AppStateType StateType
    {
        get { return _type; }
        set { _type = value; }
    }
    
    public MenuState()
    {
        _type = AppStateType.Menu;
    }
    
    public void OnEnter(IState previousState)
    {
        SceneLoader.Instance.LoadImmediately("LoadingScene");
        SceneLoader.Instance.AddRequest("UIScene", true, OnSceneUpdated, OnMainMenuLoaded);
    }

    public void Update()
    {
        
    }

    public void OnExit()
    {
        
    }
    
    private void OnMainMenuLoaded(Scene scene)
    {
        
    }

    private void OnSceneUpdated(float progress)
    {
    }
}
