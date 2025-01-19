using Newtonsoft.Json;
using System;
using System.IO;


public class JsonHelper
{
    protected JsonHelper() { }


    public static T ReadData<T>(string filePath)
    {
        string jsonRead;
        try
        {
            jsonRead = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonRead);
        }
        catch (FileNotFoundException)
        {
            return default;
        }
    }

    public static void SaveData(Object saveObject, string filePath)
    {
        string json = JsonConvert.SerializeObject(saveObject, Formatting.Indented);
        System.IO.File.WriteAllText(filePath, json);
    }
}
