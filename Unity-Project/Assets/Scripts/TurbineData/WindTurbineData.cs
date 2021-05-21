// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using UnityEngine;

/// <summary>
/// Wind Turbine Data information
/// </summary>
[Serializable]
public class WindTurbineData
{
    /// <summary>
    /// Gets or sets Turbine Id
    /// </summary>
    [field: SerializeField]
    public string TurbineId { get; set; }

    /// <summary>
    /// Gets or sets Time Interval
    /// </summary>
    [field: SerializeField]
    public string TimeInterval { get; set; }

    /// <summary>
    /// Gets or sets Event Code status of wind turbine
    /// </summary>
    [field: SerializeField]
    public int EventCode { get; set; }

    /// <summary>
    /// Gets or sets Description text of event code status
    /// </summary>
    [field: SerializeField]
    public string EventDescription { get; set; }

    /// <summary>
    /// Gets or sets Wind Speed
    /// </summary>
    [field: SerializeField]
    public double WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets Ambient temperature (C)
    /// </summary>
    [field: SerializeField]
    public double AmbientTemperature { get; set; }

    /// <summary>
    /// Gets or sets Rotor Speed (RPM)
    /// </summary>
    [field: SerializeField]
    public double RotorSpeed { get; set; }

    /// <summary>
    /// Gets or sets Power (kW)
    /// </summary>
    [field: SerializeField]
    public double Power { get; set; }
}