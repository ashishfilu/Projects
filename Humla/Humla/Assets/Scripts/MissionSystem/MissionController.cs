using System.Collections.Generic;

public delegate void OnMissionSelectedDelegate(MissionEntity selectedMission);

public class MissionController : Singleton<MissionController>
{
    public OnMissionSelectedDelegate OnMissionSelected;
    public MissionEntity Mission { get; private set; }
    
    private MissionDataManager _dataManager;
    private MissionEntityManager _entityManager;

    public MissionController()
    {
        Mission = null;
        _dataManager = new MissionDataManager();
        _entityManager = new MissionEntityManager();
        _dataManager.OnDataUpdated += _entityManager.OnDataUpdated;
    }

    ~MissionController()
    {
        _dataManager.OnDataUpdated -= _entityManager.OnDataUpdated;
        Mission = null;
    }
    
    public void AddListener(OnMissionEntitiesUpdatedDelegate onMissionEntitiesUpdated)
    {
        _entityManager.OnEntitiesUpdated += onMissionEntitiesUpdated;
    }
    public void RemoveListener(OnMissionEntitiesUpdatedDelegate onMissionEntitiesUpdated)
    {
        _entityManager.OnEntitiesUpdated -= onMissionEntitiesUpdated;
    }
    
    public void InitializeData()
    {
        _dataManager.Initialize();
    }
    
    public MissionData GetData(int index)
    {
        return _dataManager.GetData(index);
    }

    public IReadOnlyList<MissionEntity> GetAllMissions()
    {
        return _entityManager.AllMissions;
    }

    public void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < _entityManager.AllMissions.Count)
        {
            Mission = _entityManager.AllMissions[index];
            OnMissionSelected?.Invoke(Mission);
        }
    }

    public void SelectLatestUnlockedMission()
    {
        for (int i = _entityManager.AllMissions.Count-1; i >= 0 ; i--)
        {
            if (_entityManager.AllMissions[i].Locked == false)
            {
                SetCurrentIndex(i);
                return;
            }
        }
    }
    
    public void UnlockMission(int index)
    {
        _entityManager.UnlockMission(index);
    }

    public void UnlockNextMission()
    {
        for (int i = 0; i < _entityManager.AllMissions.Count-1; i++)
        {
            if (Mission == _entityManager.AllMissions[i])
            {
                _entityManager.UnlockMission(i+1);
            }
        }
    }
}

public partial class Event
{
    public static string OnMissionStarted = "OnMissionStarted";
    public static string OnMissionFinished = "OnMissionFinished";
}
