using System;
using System.Collections.Generic;
using UnityEngine;

public class InstanceData
{
    public GameObject Instance;
    public bool IsInUse;
    public String ResourcePath;
    public float IdleTime;
    
    public InstanceData(GameObject instance , string resourcePath)
    {
        Instance = instance;
        IsInUse = false;
        ResourcePath = resourcePath;
        IdleTime = 0.0f;
    }

    public void Update()
    {
        if (!IsInUse)
        {
            IdleTime += Time.deltaTime; 
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(Instance);
    }
}

public class ObjectData
{
    public String ResourcePath { get; private set; }
    private GameObject _resourceObject;
    public List<InstanceData> AllInstances { get; private set; }

    public ObjectData(string resourcePath)
    {
        ResourcePath = resourcePath;
        _resourceObject = Resources.Load(ResourcePath)as GameObject;
        _resourceObject.transform.position = new Vector3(0,1000,0);
        Debug.Assert(_resourceObject != null , $"Object missing at path :{ResourcePath}");
        AllInstances = new List<InstanceData>();
    }

    public InstanceData GetNextFreeInstance()
    {
        InstanceData output = null;
        for (int i = 0; i < AllInstances.Count; i++)
        {
            if (!AllInstances[i].IsInUse)
            {
                output = AllInstances[i];
            }
        }

        if (output == null)
        {
            GameObject instance = GameObject.Instantiate(_resourceObject);
            InstanceData temp = new InstanceData(instance,ResourcePath);
            AllInstances.Add(temp);
            output = temp;
        }
        output.IsInUse = true;
        output.Instance.SetActive(true);
        output.IdleTime = 0.0f;
        return output;
    }

    public void Update()
    {
        for (int i = AllInstances.Count-1; i >=0  ; i--)
        {
            AllInstances[i].Update();
            if (AllInstances[i].IdleTime > 5.0f)
            {
                AllInstances[i].Destroy();
                AllInstances.RemoveAt(i);
            }
        }
    }

    public void DestroyAll()
    {
        while(AllInstances.Count > 0 )
        {
            AllInstances[0].Destroy();
            AllInstances.RemoveAt(0);
        }
    }

    public void ForceDestroy(int index)
    {
        if (index >= 0 && index < AllInstances.Count)
        {
            AllInstances[index].Destroy();
            AllInstances.RemoveAt(index);
        }
    }
}

public class ObjectPool : Singleton<ObjectPool>
{
    private List<ObjectData> _allObjects;

    public ObjectPool()
    {
        _allObjects = new List<ObjectData>();
    }

    public InstanceData GetObject(string resourcePath)
    {
        for (int i = 0; i < _allObjects.Count; i++)
        {
            if (_allObjects[i].ResourcePath == resourcePath)
            {
                return _allObjects[i].GetNextFreeInstance();
            }
        }
        
        ObjectData output = new ObjectData(resourcePath);
        _allObjects.Add(output);
        return output.GetNextFreeInstance();
    }

    public void ReturnToPool(InstanceData instanceData)
    {
        instanceData.IsInUse = false;
        if (instanceData.Instance != null)
        {
            instanceData.Instance.SetActive(false);
        }
        
        instanceData.IdleTime = 0.0f;
        if (instanceData.Instance == null)//Somehow if unity destroys the game object
        {
            instanceData.IdleTime = 5.0f;//It will mark that instance as invalid and remove it from pool
        }
    }

    public void Update()
    {
        for (int i = 0; i < _allObjects.Count; i++)
        {
            _allObjects[i].Update();
        }
    }
    
    public void ClearPool()
    {
        while(_allObjects.Count > 0 )
        {
            _allObjects[0].DestroyAll();
            _allObjects.RemoveAt(0);
        }
    }
}
