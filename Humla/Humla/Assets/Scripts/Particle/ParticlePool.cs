using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class ParticlePool : Singleton<ParticlePool>
{
    private List<ObjectData> _allEmitters;
    
    public ParticlePool()
    {
        _allEmitters = new List<ObjectData>();
    }

    public InstanceData GetObject(string resourcePath)
    {
        for (int i = 0; i < _allEmitters.Count; i++)
        {
            if (_allEmitters[i].ResourcePath == resourcePath)
            {
                return _allEmitters[i].GetNextFreeInstance();
            }
        }
        
        ObjectData output = new ObjectData(resourcePath);
        _allEmitters.Add(output);
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
        for (int i = 0; i < _allEmitters.Count; i++)
        {
            ObjectData emitter = _allEmitters[i];
            for (int j = emitter.AllInstances.Count-1; j >= 0 ; j--)
            {
                if (emitter.AllInstances[j].Instance == null)
                {
                    emitter.ForceDestroy(j);
                    continue;
                }
                ParticleSystem particleSystem = emitter.AllInstances[j].Instance.GetComponent<ParticleSystem>();
                if (particleSystem == null)
                {
                    emitter.ForceDestroy(j);
                    continue;
                }
                if ( particleSystem !=null && particleSystem.IsAlive(true) == false)
                {
                    ReturnToPool(emitter.AllInstances[j]);
                }
            }
        }
    }
    
    public void ClearPool()
    {
        while(_allEmitters.Count > 0 )
        {
            _allEmitters[0].DestroyAll();
            _allEmitters.RemoveAt(0);
        }
    }
}
