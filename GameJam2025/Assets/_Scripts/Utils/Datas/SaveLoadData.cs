using UnityEngine;

public class SaveLoadData<T>
{
    void SaveData(T obj)
    {
        JsonHelper.SaveData(obj, Application.persistentDataPath + "/BubbleFight");
    }
}
