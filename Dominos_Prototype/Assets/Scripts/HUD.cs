using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Button EnvironmentButton;
    public Button GreenDominosButton;
    public Button RedDominosButton;
    public Button BallButton;


    // Start is called before the first frame update
    void Start()
    {
        GreenDominosButton.onClick.AddListener(() => { GameEventManager.Instance.TriggerEvent(EventId.KeyPad_1_Pressed, EventId.KeyPad_1_Pressed); });
        RedDominosButton.onClick.AddListener(() => { GameEventManager.Instance.TriggerEvent(EventId.KeyPad_2_Pressed, EventId.KeyPad_2_Pressed); });
        EnvironmentButton.onClick.AddListener(() => { GameEventManager.Instance.TriggerEvent(EventId.KeyPad_3_Pressed, EventId.KeyPad_3_Pressed); });
        BallButton.onClick.AddListener(() => { GameEventManager.Instance.TriggerEvent(EventId.KeyPad_4_Pressed, EventId.KeyPad_4_Pressed); });
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyUp(KeyCode.Keypad1))
        {
            GameEventManager.Instance.TriggerEvent(EventId.KeyPad_1_Pressed, EventId.KeyPad_1_Pressed);
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            GameEventManager.Instance.TriggerEvent(EventId.KeyPad_2_Pressed, EventId.KeyPad_2_Pressed);
        }
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            GameEventManager.Instance.TriggerEvent(EventId.KeyPad_3_Pressed, EventId.KeyPad_3_Pressed);
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            GameEventManager.Instance.TriggerEvent(EventId.KeyPad_4_Pressed, EventId.KeyPad_4_Pressed);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEventManager.Instance.TriggerEvent(EventId.Spacebar_Pressed, EventId.Spacebar_Pressed);
        }
    }
}

public partial class EventId
{
    public static string KeyPad_1_Pressed= "GreenButtonPressed";
    public static string KeyPad_2_Pressed = "RedButtonPressed";
    public static string KeyPad_3_Pressed = "EnvironmentButtonPressed";
    public static string Spacebar_Pressed = "BallRelease";
    public static string KeyPad_4_Pressed = "SpawnABall";
}
