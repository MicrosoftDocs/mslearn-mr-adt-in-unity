// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

/// <summary>
/// This class is used to hold data that comes from ADT
/// </summary>
[Serializable]
public class PropertyMessage
{
    public string TurbineID { get; set; }
    public bool Alert { get; set; }
}