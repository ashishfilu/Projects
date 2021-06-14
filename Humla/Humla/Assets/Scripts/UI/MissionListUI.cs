using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionListUI : MonoBehaviour
{
    public GameObject _root;

    private void Start()
    {
        MissionController.Instance.AddListener(Initialize);
        Initialize(MissionController.Instance.GetAllMissions());
    }

    private void OnDestroy()
    {
        MissionController.Instance.RemoveListener(Initialize);
    }

    public void Initialize(IReadOnlyList<MissionEntity> missionEntities)
    {
        GameObject missionIconResource = Resources.Load("Prefabs/UI/MissionIcon")as GameObject;
        for (int i = 0; i < missionEntities.Count; i++)
        {
            GameObject icon = Instantiate(missionIconResource);
            icon.transform.parent = _root.transform;
            icon.transform.localScale = Vector3.one;

            MissionIcon script = icon.GetComponent<MissionIcon>();
            script.Mission = missionEntities[i]; 
            script.Index = i;
        }
    }
}
