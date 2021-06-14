using System;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class HUD : MonoBehaviour
{
    public Button _pauseButton;
    public JoystickController _joystickController;
    public Text _score;
    public Text _multiplier;
    public Text _gameTime;
    public Progressbar _health;

    private bool _gamePaused;
    private PopUp _pausePopUp;
    
    // Start is called before the first frame update
    void Start()
    {
        _pauseButton.onClick.AddListener(OnPauseClicked);
        
        _joystickController.onMoveAlongXAxis += JoystickControllerOnOnMoveAlongXAxis;
        _joystickController.onReset += JoystickControllerOnOnReset;
        
        _gamePaused = false; 
        
        GameEventManager.Instance.SubscribeEventListener(Event.OnPlayerHealthUpdated , OnHealthUpdated);
    }

    private void OnDestroy()
    {
        _joystickController.onMoveAlongXAxis -= JoystickControllerOnOnMoveAlongXAxis;
        _joystickController.onReset -= JoystickControllerOnOnReset;
        
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnPlayerHealthUpdated , OnHealthUpdated);
        
        _pauseButton.onClick.RemoveListener(OnPauseClicked);
    }

    private void Update()
    {
        _multiplier.text = $"{GameStats.Instance.Multiplier}x";
        _score.text = $"{GameStats.Instance.Score}";
        
        GamePlayState state = AppStateMachine.Instance.CurrentState as GamePlayState;
        if (state != null )
        {
            TimeSpan duration = TimeSpan.FromSeconds(state.GameDuration);
            _gameTime.text = duration.ToString(@"mm\:ss");
        }
    }

    private void JoystickControllerOnOnReset()
    {
        if (_gamePaused)
        {
            return;
        }
        GameEventManager.Instance.TriggerEvent(Event.ArrowReleased,TranslationDirection.IDLE.ToString());
    }

    private void JoystickControllerOnOnMoveAlongXAxis(JoystickDirection direction , float delta)
    {
        if (_gamePaused)
        {
            return;
        }
        if (direction == JoystickDirection.Positive)
        {
            GameEventManager.Instance.TriggerEvent(Event.RightArrowPressed,TranslationDirection.RIGHT.ToString());
        }
        else if( direction == JoystickDirection.Negative )
        {
            GameEventManager.Instance.TriggerEvent(Event.LeftArrowPressed,TranslationDirection.LEFT.ToString());
        }
    }

    private void OnPauseClicked()
    {
        _gamePaused = true;
        GameEventManager.Instance.TriggerEvent(Event.PausePressed,_gamePaused.ToString());
        ShowGamePausedPopup();
    }

    private void OnHealthUpdated(Object data)
    {
        float ratio = float.Parse(data.ToString());
        _health.SetRatio(ratio);
    }
    
    private void ShowGamePausedPopup()
    {
        GameObject gameEndPopUp = PopUpManager.Instance.PushToStack("Prefabs/UI/TwoButtonPopUp");

        _pausePopUp = gameEndPopUp.GetComponent<PopUp>();
        _pausePopUp.SetTitle("Info!");
        _pausePopUp.SetMessage("Do you want to Quit!");
        _pausePopUp.SetButtonText("Ok",0);
        _pausePopUp.SetButtonText("Cancel",1);
        _pausePopUp.AddListenerToButton(ReturnToMainMenu,0);
        _pausePopUp.AddListenerToButton(ClosePopUp,1);
    }

    private void ReturnToMainMenu()
    {
        _pausePopUp.RemoveListenerFromButton(ReturnToMainMenu,0);
        _pausePopUp.RemoveListenerFromButton(ClosePopUp,1);
        PopUpManager.Instance.PopFromStack();
        
        GameEventManager.Instance.TriggerEvent(Event.OnMissionFinished , MissionController.Instance.Mission);
        AppStateMachine.Instance.SetCurrentState(AppStateType.Menu);
    }
    
    private void ClosePopUp()
    {
        _pausePopUp.RemoveListenerFromButton(ReturnToMainMenu,0);
        _pausePopUp.RemoveListenerFromButton(ClosePopUp,1);
        PopUpManager.Instance.PopFromStack();
        
        _gamePaused = false;
        GameEventManager.Instance.TriggerEvent(Event.PausePressed,_gamePaused.ToString());
    }
}

public partial class Event
{
    public static string LeftArrowPressed = "LeftArrowPressed";
    public static string RightArrowPressed = "RightArrowPressed";
    public static string ArrowReleased = "ArrowReleased";
    public static string RightTouchUpdated = "RightTouchUpdated";
    public static string PausePressed = "PausePressed";
}
