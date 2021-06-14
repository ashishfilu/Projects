using UnityEngine;
using UnityEngine.UI;

public class MissionIcon : MonoBehaviour
{
    public GameObject _lockIcon;
    public GameObject _border;
    public Button _button;

    public int Index { get; set; }
    public MissionEntity Mission { get; set; }


    private void Start()
    {
        _button.onClick.AddListener(OnClicked);
    }

    private void Update()
    {
        _lockIcon.SetActive(Mission.Locked);
        _border.SetActive(Mission == MissionController.Instance.Mission);
    }

    private void OnClicked()
    {
        MissionController.Instance.SetCurrentIndex(Index);
    }
}
