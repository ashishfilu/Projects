using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum ParticleType
{
    Spark,
    Explosion
}

public class ParticleDataManager : Singleton<ParticleDataManager>
{
    private Dictionary<ParticleType, string> _data;

    public ParticleDataManager()
    {
        _data = new Dictionary<ParticleType, string>();
    }

    public void Initialize()
    {
        string path = "JSONData/ParticleData";
        var textAsset = (TextAsset) Resources.Load(path);
        var doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        if (doc.DocumentElement.HasChildNodes)
        {
            XmlNode node = doc.DocumentElement.ChildNodes[0];

            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode dataNode = node.ChildNodes[i];
                ParticleType type;
                Enum.TryParse(dataNode["type"]?.InnerText, out type);
                string resourcePath = dataNode["path"]?.InnerText;

                _data.Add(type, resourcePath);
            }
        }
    }

    public string GetPath(ParticleType type)
    {
        if (_data.ContainsKey(type))
        {
            return _data[type];
        }

        return string.Empty;
    }
}
