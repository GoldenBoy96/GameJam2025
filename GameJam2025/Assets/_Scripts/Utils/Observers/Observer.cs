using System;
using System.Collections.Generic;
using UnityEngine;

public class Observer: MonoBehaviour
{
    static Dictionary<string, List<Action<object[]>>> Listeners =
        new();

    public static void AddObserver(string name, Action<object[]> callback)
    {
        if (!Listeners.ContainsKey(name))
        {
            Listeners.Add(name, new List<Action<object[]>>());
        }

        Listeners[name].Add(callback);
    }

    public static void RemoveObserver(string name, Action<object[]> callback)
    {
        if (!Listeners.ContainsKey(name))
        {
            return;
        }

        Listeners[name].Remove(callback);
    }

    public static void Notify( string name, params object[] data)
    {
        if (!Listeners.ContainsKey(name))
        {
            return;
        }

        foreach (var listener in Listeners[name])
        {
            try
            {
                listener.Invoke(data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error on invoke listener: " + ex);
            }
        }
    }

}
