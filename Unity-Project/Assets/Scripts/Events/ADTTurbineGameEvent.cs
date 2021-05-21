// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ADTTurbineGameEvent", menuName = "Scriptable Objects/Events/ADT Turbine Event")]
public class ADTTurbineGameEvent : ScriptableObject
{
    private List<ADTTurbineEventListener> listeners =
    new List<ADTTurbineEventListener>();

    public void Raise(ADTTurbineEventData eventData)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(eventData);
        }
    }

    public void RegisterListener(ADTTurbineEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(ADTTurbineEventListener listener)
    {
        listeners.Remove(listener);
    }
}

/// <summary>
/// Wrapper for data sent through Wind Turbine Events
/// </summary>
[Serializable]
public class ADTTurbineEventData
{
    public ADTTurbineEventData(WindTurbineData turbineData)
    {
        this.turbineData = turbineData;
    }

    public WindTurbineData turbineData;
}

/// <summary>
/// A UnityEvent which passes WindTurbineEventData and can be setup in the Inspector
/// </summary>
[Serializable]
public class ADTTurbineUnityEvent : UnityEvent<ADTTurbineEventData> { }