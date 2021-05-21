// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using UnityEngine;

public class TurbineScriptableObjectHolder : MonoBehaviour
{
    public List<WindTurbineScriptableObject> windTurbineDigitalTwins;

    private void Start()
    {
        if (windTurbineDigitalTwins.Capacity < 1)
        {
            windTurbineDigitalTwins = new List<WindTurbineScriptableObject>();
        }
    }
}