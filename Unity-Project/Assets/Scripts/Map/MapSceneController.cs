// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Spawns WindTurbines as children of the MapRenderer.
/// Allows focusing the MapRenderer on a turbine location.
/// </summary>
public class MapSceneController : MonoBehaviour
{
    public TurbineSiteData turbineSiteData;

    [Header("Map Settings")]
    public MapRenderer mapRenderer;

    public float mapAnimationSpeed = 25f;
    public MapSceneAnimationKind mapAnimationType = MapSceneAnimationKind.Bow;
    private float defaultZoom;

    [Header("Addressables")]
    public AssetReference turbineAssetReference;

    [Header("Events")]
    public WindTurbineGameEvent onWindTurbineLoaded;

    public WindTurbineGameEvent onWindTurbineSelected;
    public WindTurbineGameEvent onWindTurbineHoverStart;
    public WindTurbineGameEvent onWindTurbineHoverEnd;

    private void Awake()
    {
        mapRenderer.Center = turbineSiteData.SiteLocation;
        defaultZoom = mapRenderer.ZoomLevel;

        //Set the initial position of each turbine
        foreach (var turbineData in turbineSiteData.turbineData)
        {
            turbineData.CurrentLocation = new LatLon(
                turbineData.windTurbineMetaData.Latitude,
                turbineData.windTurbineMetaData.Longitude);
        }
    }

    public void SpawnTurbines()
    {
        foreach (var turbineData in turbineSiteData.turbineData)
        {
            InstantiateWindTurbineAsync(turbineData);
        }
    }

    private void InstantiateWindTurbineAsync(WindTurbineScriptableObject turbineData)
    {
        turbineAssetReference.InstantiateAsync(mapRenderer.transform).Completed += asyncHandle =>
        {
            GameObject loadedAsset = asyncHandle.Result;
            loadedAsset.name = $"Turbine - {turbineData.windTurbineData.TurbineId}";

            var turbineController = loadedAsset.GetComponent<WindTurbineController>();
            turbineController.SetTurbineData(turbineData);
            turbineController.onTurbineSelectedEvent = onWindTurbineSelected;
            turbineController.onTurbineHoverStartEvent = onWindTurbineHoverStart;
            turbineController.onTurbineHoverEndEvent = onWindTurbineHoverEnd;

            //Set the location of the MapPin
            var mapPin = loadedAsset.GetComponent<MapPin>();
            mapPin.Location = new LatLon(
                turbineData.windTurbineMetaData.Latitude,
                turbineData.windTurbineMetaData.Longitude);

            //Set the heading of the turbine
            var localRotation = new Vector3(0, (float)turbineData.windTurbineMetaData.Heading, 0);
            loadedAsset.transform.localEulerAngles = localRotation;

            turbineSiteData.AddTurbine(turbineData, loadedAsset);
            onWindTurbineLoaded.Raise(turbineData);
        };
    }

    /// <summary>
    /// Animate the MapRenderer to center on the location of a turbine
    /// </summary>
    public void CenterMapOnTurbine(WindTurbineScriptableObject turbineData)
    {
        StartCoroutine(SetMapLocation(turbineData.CurrentLocation));
    }

    /// <summary>
    /// Resets the map to center on the site location
    /// </summary>
    public void RecenterMap()
    {
        StartCoroutine(SetMapLocation(turbineSiteData.SiteLocation));
    }

    /// <summary>
    /// Called when a turbine is focused on in the UI
    /// </summary>
    public void OnFocusOnTurbineRaised(WindTurbineScriptableObject turbineData)
    {
        CenterMapOnTurbine(turbineData);
    }

    /// <summary>
    /// Returns the map and all turbines to their original positon/zoom
    /// </summary>
    public void ResetScene()
    {
        StartCoroutine(SetMapLocationAndZoom(turbineSiteData.SiteLocation, defaultZoom));
        foreach (WindTurbineScriptableObject turbineData in turbineSiteData.windTurbines.Keys)
        {
            turbineSiteData.TryGetTurbineGameObject(turbineData, out var turbineGameObject);
            var draggableMapPin = turbineGameObject.GetComponent<DraggableMapPin>();
            var latLon = new LatLon(
                turbineData.windTurbineMetaData.Latitude,
                turbineData.windTurbineMetaData.Longitude);
            draggableMapPin.SetLocation(latLon);
        }
    }

    private IEnumerator SetMapLocation(LatLon latLon)
    {
        var mapScene = new MapSceneOfLocationAndZoomLevel(latLon, mapRenderer.ZoomLevel);
        yield return mapRenderer.SetMapScene(mapScene, mapAnimationType, mapAnimationSpeed);
    }

    private IEnumerator SetMapLocationAndZoom(LatLon latLon, float zoomLevel)
    {
        var mapScene = new MapSceneOfLocationAndZoomLevel(latLon, zoomLevel);
        yield return mapRenderer.SetMapScene(mapScene, mapAnimationType, mapAnimationSpeed);
    }

    private void OnValidate()
    {
        if (turbineSiteData && mapRenderer)
        {
            mapRenderer.Center = turbineSiteData.SiteLocation;
        }
    }

#if  UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (turbineSiteData == null || mapRenderer == null)
        {
            return;
        }

        foreach (var turbineData in turbineSiteData.turbineData)
        {
            if (turbineData == null) return;

            var latLon = new LatLonAlt(
                turbineData.windTurbineMetaData.Latitude,
                turbineData.windTurbineMetaData.Longitude, 800f);
            var worldPos = mapRenderer.TransformLatLonAltToWorldPoint(latLon);
            worldPos.y = transform.position.y + mapRenderer.MapBaseHeight + 0.1f;
            Gizmos.DrawIcon(worldPos, "windturbine.png");
        }
    }

#endif
}