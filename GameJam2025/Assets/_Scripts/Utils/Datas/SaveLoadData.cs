using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadData<T>
{
    void SaveData(T obj)
    {
        JsonHelper.SaveData(obj, Application.persistentDataPath + "/BubbleFight");
    }
}
