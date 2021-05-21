// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using UnityEngine;

/// <summary>
/// Wind Turbine Meta data provides additional information on turbine.
/// </summary>
[Serializable]
public class WindTurbineMetaData
{
    /// <summary>
    /// Gets or sets Turbine facility location name
    /// </summary>
    [field: SerializeField]
    public string FacilityName { get; set; }

    /// <summary>
    /// Gets or sets Map Latitude co-ordinates
    /// </summary>
    [field: SerializeField]
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets Map Longitude co-ordinates
    /// </summary>
    [field: SerializeField]
    public double Longitude { get; set; }
    
    /// <summary>
    /// Gets or sets the Heading of the turbine
    /// 0 North, 90 East, 180 South, 270 West
    /// </summary>
    [field: SerializeField]
    public double Heading { get; set; }

    /// <summary>
    /// Gets or sets WTG Model
    /// </summary>
    [field: SerializeField]
    public string TurbineModel { get; set; }

    /// <summary>
    /// Gets or sets Hub Height in meters
    /// </summary>
    [field: SerializeField]
    public float HubHeight { get; set; }

    /// <summary>
    /// Gets or sets Rotor Diameter in meters
    /// </summary>
    [field: SerializeField]
    public float RotorDiameter { get; set; }

    /// <summary>
    /// Gets or sets Tip Height in meters
    /// </summary>
    [field: SerializeField]
    public float TipHeight { get; set; }
    
    /// <summary>
    /// Gets or sets FeederId
    /// </summary>
    [field: SerializeField]
    public int FeederId { get; set; }

    /// <summary>
    /// Gets or sets Model Id
    /// </summary>
    [field: SerializeField]
    public int ModelId { get; set; }

    /// <summary>
    /// Gets or sets the Alert status of the turbine (true or false)
    /// </summary>
    [field: SerializeField]
    public bool Alert { get; set; }
}