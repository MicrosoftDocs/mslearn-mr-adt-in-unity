// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

/// <summary>
/// This class is used to hold data that comes from ADT
/// </summary>
[Serializable]
public class TelemetryMessage
{
    public string TurbineID { get; set; }
    public string TimeInterval { get; set; }
    public string Description { get; set; }
    public int Code { get; set; }
    public double WindSpeed { get; set; }
    public double Ambient { get; set; }
    public double Rotor { get; set; }
    public double Power { get; set; }
}