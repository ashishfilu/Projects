using System;
using System.Collections.Generic;
using System.Xml;

public enum VehicleType
{
    Tank,
    Helicopter,
    SpiderBot,
    Spaceship,
}

public enum ArmorType
{
    Bullet,
    Rocket,
}

public class FiringData
{
    public ArmorType Type;
    public float LinearFiringRate;
    public float RadialFiringRate;
    public int NumberOfBulletsForLinearFire;
    public int NumberOfBulletsForMultipleFire;
    public int NumberOfBulletsForRadialFire;
    public int BaseDamageUponImpact;

    public FiringData()
    {
        Type = ArmorType.Bullet;
        LinearFiringRate = RadialFiringRate = 1.0f;
        NumberOfBulletsForLinearFire = NumberOfBulletsForMultipleFire = NumberOfBulletsForRadialFire = 1;
        BaseDamageUponImpact = 0;
    }
}

public class VehicleData
{
    public string Id;
    public float Speed;
    public float MaxHealth;
    public VehicleType Type;
    public string IconPath;
    public string PrefabPath;
    public List<Currency> Cost;
    public List<FiringData> FiringData;

    public VehicleData()
    {
        Speed = 0.0f;
        MaxHealth = 0.0f;
        Type = VehicleType.Tank;
        Cost = new List<Currency>();
        FiringData = new List<FiringData>();
    }

    public void SetData(XmlNode dataNode)
    {
        Id = dataNode["id"]?.InnerText;
        MaxHealth = float.Parse(dataNode["maxHealth"]?.InnerText);
        Speed = float.Parse(dataNode["speed"]?.InnerText);
        string vehicleType = dataNode["vehicleType"]?.InnerText;
        Enum.TryParse(vehicleType, out Type);
        IconPath = dataNode["icon"]?.InnerText;
        PrefabPath = dataNode["prefab"]?.InnerText;

        XmlNode costNode = dataNode["cost"];
        if (costNode != null)
        {
            for (var i = 0; i < costNode.ChildNodes.Count; i++)
            {
                XmlNode childNode = costNode.ChildNodes[i];
                
                CurrencyType currencyType;
                string currencyTypeString = childNode["type"]?.InnerText;
                long amount = long.Parse(childNode["amount"]?.InnerText);
                Enum.TryParse(currencyTypeString, out currencyType);
                Cost.Add(new Currency(currencyType,amount));
            }
        }
        
        XmlNode firingNode = dataNode["firingData"];
        for (var i = 0; i < firingNode.ChildNodes.Count; i++)
        {
            XmlNode childNode = firingNode.ChildNodes[i];
            
            FiringData firingData = new FiringData();
            Enum.TryParse(childNode["type"]?.InnerText , out firingData.Type);
            firingData.LinearFiringRate = float.Parse(childNode["linearFiringRate"]?.InnerText);
            firingData.RadialFiringRate = float.Parse(childNode["radialFiringRate"]?.InnerText);
            firingData.NumberOfBulletsForLinearFire = int.Parse(childNode["numberOfBulletsForLinearFire"]?.InnerText);
            firingData.NumberOfBulletsForMultipleFire = int.Parse(childNode["numberOfBulletsForMultipleFire"]?.InnerText);
            firingData.NumberOfBulletsForRadialFire= int.Parse(childNode["numberOfBulletsForRadialFire"]?.InnerText);
            firingData.BaseDamageUponImpact = int.Parse(childNode["baseDamageUponImpact"]?.InnerText);
            
            FiringData.Add(firingData);
        }
    }

    public FiringData GetFiringData(ArmorType type)
    {
        for (int i = 0; i < FiringData.Count; i++)
        {
            if (FiringData[i].Type == type)
            {
                return FiringData[i];
            }
        }

        return null;
    }
}

