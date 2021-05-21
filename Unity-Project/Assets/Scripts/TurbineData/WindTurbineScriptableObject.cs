// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Geospatial;
using UnityEngine;

/// <summary>
/// Scriptable Object for Wind Turbine data received from ADT
/// </summary>
[CreateAssetMenu(fileName = "TurbineData", menuName = "Scriptable Objects/Turbine Data/Turbine Data")]
public class WindTurbineScriptableObject : ScriptableObject
{
    /// <summary>
    /// onDataUpdated action
    /// </summary>
    public Action onDataUpdated;

    /// <summary>
    /// Wind turbine data
    /// </summary>
    public WindTurbineData windTurbineData;

    /// <summary>
    /// Wind Turbine Meta Data
    /// </summary>
    public WindTurbineMetaData windTurbineMetaData;

    /// <summary>
    /// Update scriptable object data and invoke defined action.
    /// </summary>
    /// <param name="newWindTurbineData"></param>
    public void UpdateData(WindTurbineData newWindTurbineData)
    {
        windTurbineData = newWindTurbineData;
        onDataUpdated?.Invoke();
    }

    /// <summary>
    /// Stores the Wind Turbines user placed location in the scene
    /// </summary>
    public LatLon CurrentLocation { get; set; }
}