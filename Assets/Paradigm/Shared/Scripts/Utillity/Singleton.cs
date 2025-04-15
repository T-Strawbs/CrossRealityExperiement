﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Orignial Author: https://github.com/dilmerv
 * Link: https://github.com/dilmerv/UnityMultiplayerPlayground/blob/master/Assets/Scripts/Core/NetworkSingleton.cs
 */

/// <summary>
/// Base class for objects that need to be static and accessed globally.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour
        where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsByType(typeof(T),FindObjectsSortMode.None) as T[];
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