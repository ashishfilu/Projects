using UnityEngine;

public class Utils 
{
    public static Vector3 Vector3FromString(string data)
    {
        if (data.StartsWith ("(") && data.EndsWith (")")) 
        {
            data = data.Substring(1, data.Length-2);
        } 
        string[] sArray = data.Split(',');

        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));
 
        return result;
    }
}
