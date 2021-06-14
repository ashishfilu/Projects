using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        DataManager.Instance.LoadAllDataManagers();
        User.Instance.Load();
        AppStateMachine.Instance.InitializeGameStates();
        
        //Set Menu as initial state
        AppStateMachine.Instance.SetCurrentState(AppStateType.Menu);
    }
    
    void Update()
    {
        SceneLoader.Instance.Update(Time.deltaTime);
        AppStateMachine.Instance.Update();
    }
}

public class MainInstance
{
    public static Main Instance
    {
        get
        {
            GameObject gameObject = GameObject.Find("MainPrefab");
            return gameObject.GetComponent<Main>();
        }
    }
}


