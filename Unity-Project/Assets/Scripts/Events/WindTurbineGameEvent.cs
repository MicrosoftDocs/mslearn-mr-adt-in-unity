// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A Scriptable Object used to pass Wind Turbine events between the scene and UI
/// </summary>
[CreateAssetMenu(fileName = "WindTurbineGameEvent", menuName = "Scriptable Objects/Events/Wind Turbine Game Event")]
public class WindTurbineGameEvent : ScriptableObject
{
    private List<WindTurbineEventListener> listeners =
        new List<WindTurbineEventListener>();

    public void Raise(WindTurbineScriptableObject turbineData)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            if (listeners[i] != null)
            {
                listeners[i].OnEventRaised(turbineData);
            }
        }
    }

    public void AddListener(WindTurbineEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(WindTurbineEventListener listener)
    {
        listeners.Remove(listener);
    }
}

/// <summary>
/// A UnityEvent which passes WindTurbineEventData and can be setup in the Inspector
/// </summary>
[Serializable]
public class WindTurbineUnityEvent : UnityEvent<WindTurbineScriptableObject> { }