// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;

/// <summary>
/// Adds position indicators to the edge of the MapRenderer. 
/// </summary>
public class MapPinIndicatorController : MonoBehaviour
{
    public TurbineSiteData siteData;
    public MapRenderer mapRenderer;
    public MapPinIndicator indicatorPrefab;
    
    public float indicatorRadius = 1f;
    public float indicatorHeight = 0.2f;

    private Dictionary<MapPin, MapPinIndicator> indicators = new Dictionary<MapPin, MapPinIndicator>();

    /// <summary>
    /// Creates a map indicator when a wind turbine is instantiated in the scene.
    /// </summary>
    public void OnWindTurbineAdded(WindTurbineScriptableObject turbineData)
    {
        if (siteData.TryGetTurbineGameObject(turbineData, out var turbine))
        {
            MapPin mapPin = turbine.GetComponent<MapPin>();
            var indicator = Instantiate(indicatorPrefab, transform);
            indicator.Setup(turbineData);
            indicators.Add(mapPin, indicator);
        }
    }
    
    private void Start()
    {
        var childTurbines = mapRenderer.GetComponentsInChildren<DraggableMapPin>(true);
        foreach (var turbine in childTurbines)
        {
            var indicator = Instantiate(indicatorPrefab, transform);
            indicators.Add(turbine.LinkedMapPin, indicator);
        }
    }

    private void Update()
    {
        UpdateIndicatorPositions();
    }

    private void UpdateIndicatorPositions()
    {
        foreach (var mapPin in indicators.Keys)
        {
            var indicator = indicators[mapPin];
            
            //Show the indicator if the MapPin is currently out of view
            if(mapPin.gameObject.activeSelf == false)
            {
                indicator.gameObject.SetActive(true);
                GetPositionOnMapEdge(mapPin, out Vector3 pos, out Quaternion rot);
                pos = indicator.HasAlert ? pos + new Vector3(0, 0.05f, 0) : pos;
                indicator.transform.SetPositionAndRotation(pos, rot);
            }
            else
            {
                indicator.gameObject.SetActive(false);
            }
        }
    }

    private void GetPositionOnMapEdge(MapPin mapPin, out Vector3 position, out Quaternion rotation)
    {
        //Convert the MapPin location to a world position
        Vector3 targetPos = mapRenderer.TransformMercatorWithAltitudeToWorldPoint(
            mapPin.Location.ToMercatorCoordinate(),
            mapPin.Altitude);
        targetPos.y = mapRenderer.transform.position.y;

        //Get the direction from map centre to the point
        Vector3 dir = targetPos - mapRenderer.transform.position ;

        //Move the point in the direction of the MapPin on the edge of the map
        float mapScale = mapRenderer.transform.localScale.magnitude;
        position = mapRenderer.transform.position + (dir.normalized * (indicatorRadius * mapScale));
        position.y = mapRenderer.transform.position.y + (indicatorHeight * mapScale);
        rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Draw a preview of where out of view MapPins indicators will sit
        var handleColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.cyan;
        float mapScale = mapRenderer.transform.localScale.magnitude;
        var rot = Quaternion.LookRotation(transform.up, transform.up);
        var pos = mapRenderer.transform.position;
        pos.y += indicatorHeight * mapScale;
        UnityEditor.Handles.CircleHandleCap(0, pos, rot, indicatorRadius * mapScale, EventType.Repaint);
        UnityEditor.Handles.color = handleColor;
    }
#endif
}
