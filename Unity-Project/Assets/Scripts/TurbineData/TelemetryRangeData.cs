// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Scriptable object for Telemetry range data
/// </summary>
[CreateAssetMenu(fileName = "TelemetryRangeData", menuName = "Scriptable Objects/Turbine Data/Telemetry Range Data")]
public class TelemetryRangeData : ScriptableObject
{
    /// <summary>
    /// Minimum Value
    /// </summary>
    public double minValue;

    /// <summary>
    /// Maximum Value
    /// </summary>
    public double maxValue;

    /// <summary>
    /// Optimal operating value
    /// </summary>
    public double optimalValue;
    
    /// <summary>
    /// Gradient indicating the health of a value within the telemetry range
    /// </summary>
    public Gradient healthIndicatorGradient;

    /// <summary>
    /// Display Units
    /// </summary>
    public string units;
    
    /// <summary>
    /// Display Rounding Precision
    /// </summary>
    public int displayRoundingPrecision;
}