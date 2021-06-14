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
    private List<InstanceData> _allInstances;

    public ObjectData(string resourcePath)
    {
        ResourcePath = resourcePath;
        _resourceObject = Resources.Load(ResourcePath)as GameObject;
        Debug.Assert(_resourceObject != null , $"Object missing at path :{ResourcePath}");
        _allInstances = new List<InstanceData>();
    }

    public InstanceData GetNextFreeInstance()
    {
        InstanceData output = null;
        for (int i = 0; i < _allInstances.Count; i++)
        {
            if (!_allInstances[i].IsInUse)
            {
                output = _allInstances[i];
            }
        }

        if (output == null)
        {
            GameObject instance = GameObject.Instantiate(_resourceObject);
            InstanceData temp = new InstanceData(instance,ResourcePath);
            _allInstances.Add(temp);
            output = temp;
        }
        output.IsInUse = true;
        output.Instance.SetActive(true);
        output.IdleTime = 0.0f;
        return output;
    }

    public void Update()
    {
        for (int i = _allInstances.Count-1; i >=0  ; i--)
        {
            _allInstances[i].Update();
            if (_allInstances[i].IdleTime > 5.0f)
            {
                _allInstances[i].Destroy();
                _allInstances.RemoveAt(i);
            }
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
        instanceData.Instance?.SetActive(false);
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
}
