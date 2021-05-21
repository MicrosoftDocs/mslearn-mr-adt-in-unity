// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

/// <summary>
/// Sends events to the UI when this turbine is interacted with.
/// Syncs the rotor speed of the turbine with the ADT data.
/// </summary>
public class WindTurbineController : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
{
    private static readonly int ColorShaderID = Shader.PropertyToID("_Color");

    [SerializeField]
    private WindTurbineScriptableObject turbineData;

    [Header("Alert Status")]
    [SerializeField]
    private GameObject alertIndicator;

    [SerializeField]
    private Material turbineMaterial;

    [SerializeField]
    private Color alertOverrideColor;

    private MaterialPropertyBlock colorOverride;
    private MeshRenderer[] turbineMeshes = new MeshRenderer[0];

    private RotateRPM rotateRpm;
    private MapRenderer mapRenderer;

    [Header("Events")]
    public WindTurbineGameEvent onTurbineSelectedEvent;

    public WindTurbineGameEvent onTurbineHoverStartEvent;
    public WindTurbineGameEvent onTurbineHoverEndEvent;

    private void Awake()
    {
        rotateRpm = GetComponentInChildren<RotateRPM>();
        mapRenderer = GetComponentInParent<MapRenderer>();
        colorOverride = new MaterialPropertyBlock();
    }

    private void Start()
    {
        //Find all MeshRenderers using the Turbine material.
        turbineMeshes = GetComponentsInChildren<MeshRenderer>()
            .Where(mr => mr.sharedMaterials[0] == turbineMaterial)
            .ToArray();

        if (turbineData)
        {
            SetTurbineData(turbineData);
        }
    }

    public void SetTurbineData(WindTurbineScriptableObject data)
    {
        if (turbineData != null)
        {
            turbineData.onDataUpdated -= OnTurbineDataUpdated;
        }

        turbineData = data;
        turbineData.onDataUpdated += OnTurbineDataUpdated;
        turbineData.CurrentLocation = new LatLon(
            data.windTurbineMetaData.Latitude,
            data.windTurbineMetaData.Longitude);
        OnTurbineDataUpdated();
    }

    public void SetAlertStatus(bool showAlert)
    {
        alertIndicator.SetActive(showAlert);
        var color = showAlert ? alertOverrideColor : turbineMaterial.GetColor(ColorShaderID);
        colorOverride.SetColor(ColorShaderID, color);
        foreach (var turbineMesh in turbineMeshes)
        {
            turbineMesh.SetPropertyBlock(colorOverride);
        }
    }

    private void OnDestroy()
    {
        if (turbineData != null)
        {
            turbineData.onDataUpdated -= OnTurbineDataUpdated;
        }
    }

    private void Update()
    {
        if (!turbineData)
            return;

        var localPos = mapRenderer.transform.InverseTransformPoint(transform.position);
        var coordinate = mapRenderer.TransformLocalPointToMercator(localPos);
        turbineData.CurrentLocation = coordinate.ToLatLon();
    }

    private void OnTurbineDataUpdated()
    {
        rotateRpm.rpm = (float)turbineData.windTurbineData.RotorSpeed;
        SetAlertStatus(turbineData.windTurbineMetaData.Alert);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        onTurbineHoverStartEvent.Raise(turbineData);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        onTurbineHoverEndEvent.Raise(turbineData);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        onTurbineSelectedEvent.Raise(turbineData);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }
}