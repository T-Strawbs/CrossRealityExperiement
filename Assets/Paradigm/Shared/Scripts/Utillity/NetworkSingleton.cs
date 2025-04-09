using Unity.Netcode;
using UnityEngine;

/*
 * Orignial Author: https://github.com/dilmerv
 * Link: https://github.com/dilmerv/UnityMultiplayerPlayground/blob/master/Assets/Scripts/Core/NetworkSingleton.cs
 */
/// <summary>
/// Base class for objects that need to be static and accessed globally while needing to be a NetworkBehaviour.
/// </summary>
/// <typeparam name="T"></typeparam>
public class NetworkSingleton<T> : NetworkBehaviour
        where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = string.Format("_{0}", typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}