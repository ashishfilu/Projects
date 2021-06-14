
using System.Collections.Generic;

public class MissionEntity
{
    public MissionData Data { get; private set; }
    public bool Locked { get; set; }

    public MissionEntity(MissionData data)
    {
        Data = data;
        Locked = true;
    }
}

public delegate void OnMissionEntitiesUpdatedDelegate(IReadOnlyList<MissionEntity> runtimeEntities);

public class MissionEntityManager
{
    public event OnMissionEntitiesUpdatedDelegate OnEntitiesUpdated;
    
    public List<MissionEntity> AllMissions { get; private set; }

    public MissionEntityManager()
    {
        AllMissions = new List<MissionEntity>();
    }

    public void OnDataUpdated(IReadOnlyList<MissionData> data)
    {
        AllMissions.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            MissionEntity entity = new MissionEntity(data[i]);
            AllMissions.Add(entity);
        }
    }

    public void UnlockMission(int index)
    {
        if (index >= 0 && index < AllMissions.Count)
        {
            AllMissions[index].Locked = false;
        }
    }
}
