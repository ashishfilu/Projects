using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnBackbuttonPressedDelegate();

public class MenuUI : MonoBehaviour
{
    public Button Mission;
    public Button Play;
    public Button Settings;
    public Button Shop;
    public Button Credits;
    public Button BackButton;
    public List<GameObject> _uiStack;
    
    private int _index = 0;

    void Start()
    {
        Mission.onClick.AddListener(OnMissionButtonClicked);
        Play.onClick.AddListener(OnPlayClicked);
        Settings.onClick.AddListener(OnSettingsClicked);
        Shop.onClick.AddListener(OnShopClicked);
        Credits.onClick.AddListener(OnCreditsClicked);
        BackButton.onClick.AddListener(OnBackbuttonPressed);
        UpdateScreen();
        
        MissionController.Instance.OnMissionSelected += OnMissionSelected;
    }

    private void OnDestroy()
    {
        Mission.onClick.RemoveListener(OnMissionButtonClicked);
        Play.onClick.RemoveListener(OnPlayClicked);
        Settings.onClick.RemoveListener(OnSettingsClicked);
        BackButton.onClick.RemoveListener(OnBackbuttonPressed);
        
        MissionController.Instance.OnMissionSelected -= OnMissionSelected;
    }

    private void OnMissionSelected(MissionEntity selectedMission)
    {
        if (selectedMission.Locked)
        {
            Play.interactable = false;
        }
        else
        {
            Play.interactable = true;
        }
    }

    private void OnMissionButtonClicked()
    {
        _index = 1;
        UpdateScreen();
        MissionController.Instance.SelectLatestUnlockedMission();
    }
    
    private void OnPlayClicked()
    {
        AppStateMachine.Instance.SetCurrentState(AppStateType.GamePlay);
    }

    private void OnSettingsClicked()
    {

    }

    private void OnCreditsClicked()
    {
        
    }
    
    private void OnBackbuttonPressed()
    {
        _index = 0;
        UpdateScreen();
    }

    private void OnShopClicked()
    {
        _index = 2;
        UpdateScreen();
    }
    
    private void UpdateScreen()
    {
        for (int i = 0; i < _uiStack.Count; i++)
        {
            _uiStack[i].SetActive(false);
        }
        _uiStack[_index].SetActive(true);
    }
}


