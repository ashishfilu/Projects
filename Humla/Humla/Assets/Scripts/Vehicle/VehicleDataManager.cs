using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public delegate void OnVehicleDataUpdatedDelegate(IReadOnlyList<VehicleData> data);

public class VehicleDataManager 
{
    public event OnVehicleDataUpdatedDelegate OnDataUpdated;
    
    public List<VehicleData> Data { get; private set; }

    public VehicleDataManager()
    {
        Data = new List<VehicleData>();
    }

    public void Initialize()
    {
        string path = "JSONData/VehicleData";
        var textAsset = (TextAsset) Resources.Load(path);
        var doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        if (doc.DocumentElement.HasChildNodes)
        {
            XmlNode node = doc.DocumentElement.ChildNodes[0];
            
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode dataNode = node.ChildNodes[i];
                if( dataNode.Name != "vehicleData")
                {
                    continue;
                }
                
                VehicleData temp = new VehicleData();
                temp.SetData(dataNode);
                Data.Add(temp);
            }
        }
        
        OnDataUpdated?.Invoke(Data);
    }

    public VehicleData GetData(VehicleType type)
    {
        for (int i = 0; i < Data.Count; i++)
        {
            if (Data[i].Type == type)
            {
                return Data[i];
            }
        }

        return null;
    }

    public VehicleData GetData(string id)
    {
        for (int i = 0; i < Data.Count; i++)
        {
            if (Data[i].Id == id)
            {
                return Data[i];
            }
        }

        return null;
    }
    
    public VehicleData GetData(int index)
    {
        if (index >= 0 && index < Data.Count)
        {
            return Data[index];
        }

        return null;
    }
    
    public int Count()
    {
        return Data.Count;
    }
}
