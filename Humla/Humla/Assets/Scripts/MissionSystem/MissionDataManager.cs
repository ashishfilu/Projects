
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionData
{
    public string Path;
    public float Duration;
    public string AmbientColor;

    public Vector3 GetAmbientColor()
    {
        return Utils.Vector3FromString(AmbientColor);
    }
}

[Serializable]
public class MissionList
{
    public List<MissionData> Missions = new List<MissionData>();
}

public delegate void OnMissionDataUpdatedDelegate(IReadOnlyList<MissionData> data);

public class MissionDataManager 
{
    public event OnMissionDataUpdatedDelegate OnDataUpdated;
    
    private MissionList _data;
    
    public MissionDataManager()
    {
        _data = new MissionList();
    }

    public void Initialize()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JSONData/MissionData");
        string jsonString = textAsset.text;
        _data = JsonUtility.FromJson<MissionList>(jsonString);
        OnDataUpdated?.Invoke(_data.Missions);
    }

    public MissionData GetData(int index)
    {
        if (index >= 0 && index <= _data.Missions.Count)
        {
            return _data.Missions[index];
        }

        return null;
    }

    public int Count()
    {
        return _data.Missions.Count;
    }
}
