using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class User : Singleton<User>
{
    private UserInventory _inventory;

    public User()
    {
        _inventory = new UserInventory();
    }

    public bool Purchase(VehicleEntity vehicleEntity)
    {
        return _inventory.Purchase(vehicleEntity);
    }

    public bool DoesInventoryExits(VehicleEntity vehicleEntity)
    {
        return _inventory.DoesExists(vehicleEntity);
    }
    
    public void Save()
    {
        string filePath = Application.persistentDataPath + "/PlayerProfile.xml";
        
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("profile");
        xml.AppendChild(root);
        
        XmlElement data = xml.CreateElement("data");
        
        XmlElement missionData = xml.CreateElement("mission");
        IReadOnlyList<MissionEntity> allMissions = MissionController.Instance.GetAllMissions();
        for (int i = 0; i < allMissions.Count; i++)
        {
            XmlElement childNode = xml.CreateElement("data");
            
            XmlElement index = xml.CreateElement("index");
            index.InnerText = i.ToString();
            childNode.AppendChild(index);
            
            XmlElement status = xml.CreateElement("status");
            status.InnerText = allMissions[i].Locked.ToString();
            childNode.AppendChild(status);

            missionData.AppendChild(childNode);
        }
        data.AppendChild(missionData);
        
        XmlElement inventory = xml.CreateElement("inventory");
        for (int i = 0; i < _inventory.Vehicles.Count; i++)
        {
            XmlElement childNode = xml.CreateElement("data");
            childNode.InnerText = _inventory.Vehicles[i].Data.Id;
            inventory.AppendChild(childNode);
        }

        data.AppendChild(inventory);
        
        XmlElement currencyNode = xml.CreateElement("currency");
        for (int i = 0; i < (int) CurrencyType.Max; i++)
        {
            XmlElement currencyData = xml.CreateElement("data");
            
            Currency currency = CurrencyController.Instance.GetCurrency((CurrencyType) i);
            
            XmlElement type = xml.CreateElement("type");
            type.InnerText = ((CurrencyType) i).ToString();
            currencyData.AppendChild(type);
            
            XmlElement amount = xml.CreateElement("amount");
            amount.InnerText = currency.Amount.ToString();
            currencyData.AppendChild(amount);
            
            currencyNode.AppendChild(currencyData);
        }

        data.AppendChild(currencyNode);
        root.AppendChild(data);

        string xmlString = xml.OuterXml;
        File.WriteAllText(filePath,xmlString);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/PlayerProfile.xml";
        if (!File.Exists(path))
        {
            CreateDefaultProfile();
            return;
        }
        string data = File.ReadAllText(path);
        var doc = new XmlDocument();
        doc.LoadXml(data);
        if (doc.DocumentElement.HasChildNodes)
        {
            XmlNode dataNode = doc.DocumentElement.ChildNodes[0];
            
            XmlNode missionNode = dataNode["mission"];
            for (var i = 0; i < missionNode?.ChildNodes.Count; i++)
            {
                XmlNode childNode = missionNode.ChildNodes[i];
                int index = int.Parse(childNode["index"]?.InnerText);
                bool status = bool.Parse(childNode["status"]?.InnerText);

                MissionController.Instance.GetAllMissions()[index].Locked = status;
            }
            
            XmlNode inventoryNode = dataNode["inventory"];
            for (var i = 0; i < inventoryNode.ChildNodes.Count; i++)
            {
                string id = inventoryNode.ChildNodes[i]?.InnerText;
                _inventory.AddToInventory(id);
            }

            XmlNode currencyNode = dataNode["currency"];
            for (var i = 0; i < currencyNode.ChildNodes.Count; i++)
            {
                XmlNode node = currencyNode.ChildNodes[i];
                CurrencyType currencyType;
                Enum.TryParse(node["type"]?.InnerText, out currencyType);
                long amount = long.Parse(node["amount"].InnerText);
                
                CurrencyController.Instance.SetAmount(currencyType,amount);
            }
        }
    }

    public void SetDefaultSelectedVehicle()
    {
        VehicleController.Instance.SetSelectedVehicle(_inventory.Vehicles[0].Data.Id);
    }
    
    private void CreateDefaultProfile()
    {
        CurrencyController.Instance.AddDelta(CurrencyType.Coins,300);
        VehicleEntity defaultVehicle = VehicleController.Instance.GetVehicles(VehicleType.Tank)[0];
        _inventory.AddToInventory(defaultVehicle);
        MissionController.Instance.GetAllMissions()[0].Locked = false;
        Save();
    }
    
}
