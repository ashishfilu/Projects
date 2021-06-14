using System.Collections.Generic;
using UnityEngine;

public enum EmissionType
{
    SINGLE=0,
    MULTIPLE,
    MULTIPLE_RADIAL
}

public struct CurrentFiringData
{
    public EmissionType emissionType;
    public ArmorType armorType;
    public int count;
    public Vector3 startPosition;
    public Vector3 direction;
    public int damageUponImpact;
    public VehicleType source;
};

public class BulletEmitter
{
    private GameObject _bulletResource;
    private GameObject _root;
    private float _linearSpeed , _radialSpeed;
    private List<InstanceData> _allBullets;
    private List<InstanceData> _allRockets;   
    
    public BulletEmitter()
    {
        _allBullets = new List<InstanceData>();
        _allRockets = new List<InstanceData>();
        _root = GameObject.FindGameObjectWithTag("GameEnvironmentRoot");
        _linearSpeed = 500.0f;
        _radialSpeed = 400.0f;
    }

    public void Update()
    {
        for (int i = _allBullets.Count - 1; i >= 0; i--)
        {
            if (_allBullets[i].Instance.activeSelf == false)
            {
                ObjectPool.Instance.ReturnToPool(_allBullets[i]);
                _allBullets.RemoveAt(i);
            }
        }
        for (int i = _allRockets.Count - 1; i >= 0; i--)
        {
            if (_allRockets[i].Instance.activeSelf == false)
            {
                ObjectPool.Instance.ReturnToPool(_allRockets[i]);
                _allRockets.RemoveAt(i);
            }
        }
    }
    
    public void Fire(CurrentFiringData currentFiringData)
    {
        string resourcePath = "Prefabs/Armor/Bullet";
        if (currentFiringData.armorType == ArmorType.Rocket)
        {
            resourcePath = "Prefabs/Armor/Rocket";
        }
        
        if (currentFiringData.emissionType == EmissionType.SINGLE)
        {
            InstanceData bullet = ObjectPool.Instance.GetObject(resourcePath);
            bullet.Instance.transform.parent = _root.transform;
            
            bullet.Instance.transform.position = currentFiringData.startPosition + currentFiringData.direction * 20.0f;
            bullet.Instance.transform.right = currentFiringData.direction;

            Bullet script = bullet.Instance.GetComponent<Bullet>();
            script.DamageOnHit = currentFiringData.damageUponImpact;
            script.Source = currentFiringData.source;
            
            bullet.Instance.SetActive(true);
            _allBullets.Add(bullet);
        }
        else if (currentFiringData.emissionType == EmissionType.MULTIPLE)
        {
            float offset = 20;
            for (int i = 0; i < currentFiringData.count; i++)
            {
                InstanceData bullet = ObjectPool.Instance.GetObject(resourcePath);
                bullet.Instance.transform.parent = _root.transform;
            
                bullet.Instance.transform.position = currentFiringData.startPosition + currentFiringData.direction * offset;
                bullet.Instance.transform.right = currentFiringData.direction;

                Bullet script = bullet.Instance.GetComponent<Bullet>();
                script.DamageOnHit = currentFiringData.damageUponImpact;
                script.Source = currentFiringData.source;
                
                bullet.Instance.SetActive(true);
                _allBullets.Add(bullet);
            }
            
        }
        else if (currentFiringData.emissionType == EmissionType.MULTIPLE_RADIAL)
        {
            float startAngle = -45.0f;
            float endAngle = 45.0f;
            float offsetAngle = (endAngle - startAngle) / currentFiringData.count;
            
            for (int i = 0; i < currentFiringData.count; i++)
            {
                InstanceData bullet = ObjectPool.Instance.GetObject(resourcePath);
                bullet.Instance.transform.parent = _root.transform;

                Vector3 lookDirection = Quaternion.Euler(0, 0, startAngle) * currentFiringData.direction ;
                
                bullet.Instance.transform.position = currentFiringData.startPosition + lookDirection * 20.0f;
                bullet.Instance.transform.right = lookDirection;

                Bullet script = bullet.Instance.GetComponent<Bullet>();
                script.DamageOnHit = currentFiringData.damageUponImpact;
                script.Source = currentFiringData.source;
                
                startAngle += offsetAngle;
                
                bullet.Instance.SetActive(true);
                _allBullets.Add(bullet);
            }
        }
    }

    public void DropRocket(CurrentFiringData currentFiringData)
    {
        InstanceData rocket = ObjectPool.Instance.GetObject("Prefabs/Armor/Rocket");
        rocket.Instance.transform.parent = _root.transform;
            
        rocket.Instance.transform.position = currentFiringData.startPosition + currentFiringData.direction * 20.0f;
        rocket.Instance.transform.right = currentFiringData.direction;

        Rocket script = rocket.Instance.GetComponent<Rocket>();
        script.RightVector = currentFiringData.direction;
        script.DamageOnHit = currentFiringData.damageUponImpact;
        script.Source = currentFiringData.source;
        
        rocket.Instance.SetActive(true);
        _allRockets.Add(rocket);
    }
}
