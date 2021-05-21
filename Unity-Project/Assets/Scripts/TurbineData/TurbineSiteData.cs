// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "TurbineSiteData", menuName = "Scriptable Objects/Turbine Data/Turbine Site Data", order = 1)]
public class TurbineSiteData : ScriptableObject
{
    [Header("Site Info")]
    public string facilityName;

    [SerializeField]
    private LatLonWrapper siteLatLon;

    /// <summary>
    /// The Latitude/Longitude to center the map
    /// </summary>
    public LatLon SiteLocation => siteLatLon.ToLatLon();

    public WindTurbineScriptableObject[] turbineData;

    public Dictionary<WindTurbineScriptableObject, GameObject> windTurbines =
        new Dictionary<WindTurbineScriptableObject, GameObject>();

    public void AddTurbine(WindTurbineScriptableObject data, GameObject gameObject)
    {
        windTurbines.Add(data, gameObject);
    }

    /// <summary>
    /// Find the Wind Turbine GameObject related to a turbine data.
    /// </summary>
    public bool TryGetTurbineGameObject(WindTurbineScriptableObject data, out GameObject turbineObject)
    {
        return windTurbines.TryGetValue(data, out turbineObject);
    }
}