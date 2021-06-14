using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    public Button SpawnButton;
    public Button RotatorCWButton;
    public Button RotatorCCWButton;
    public Button AIBotButton;
    public Button LoadButton;
    public Button SaveButton;
    public Button SimulateButton;

    private void Start()
    {
        SpawnButton.onClick.AddListener(OnSpawnButtonClicked);
        RotatorCWButton.onClick.AddListener(OnRotatorCWButtonClicked);
        RotatorCCWButton.onClick.AddListener(OnRotatorCCWButtonClicked);
        AIBotButton.onClick.AddListener(OnAIButtonClicked);
        LoadButton.onClick.AddListener(OnLoadButtonClicked);
        SaveButton.onClick.AddListener(OnSaveButtonClicked);
        SimulateButton.onClick.AddListener(OnSimulateButtonClicked);
    }

    private void OnSpawnButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnSpawnerButtonClicked);
    }
    private void OnRotatorCWButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnRotatorCWButtonClicked);
    }
    private void OnRotatorCCWButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnRotatorCCWButtonClicked);
    }
    private void OnAIButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnAIBotButtonClicked);
    }
    private void OnLoadButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnLoadButtonClicked);
    }
    private void OnSaveButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnSaveButtonClicked);
    }
    private void OnSimulateButtonClicked()
    {
        GameEventManager.Instance.TriggerEvent(GameEventIDs.OnSimulateButtonClicked);
    }
}

public partial class GameEventIDs
{
    public static string OnSpawnerButtonClicked = "SpawnerButtonClicked";
    public static string OnRotatorCWButtonClicked = "RotatorCWButtonClicked";
    public static string OnRotatorCCWButtonClicked = "RotatorCCWButtonClicked";
    public static string OnAIBotButtonClicked = "AIButtonClicked";
    public static string OnLoadButtonClicked = "LoadButtonClicked";
    public static string OnSaveButtonClicked = "SaveButtonClicked";
    public static string OnSimulateButtonClicked = "SimulateButtonClicked";
}
