// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Controls the zoom level of a MapRenderer based on its minimum and maximum zoom levels
/// </summary>
[RequireComponent(typeof(PinchSlider))]
public class MapZoomSlider : MonoBehaviour
{
    public MapRenderer mapRenderer;
    private PinchSlider slider;
    private bool isInteracting;
    private float defaultZoom;

    public void Awake()
    {
        slider = GetComponent<PinchSlider>();
        defaultZoom = mapRenderer.ZoomLevel;
        ResetZoom();
    }
    
    public void OnZoomSliderUpdated(SliderEventData eventData)
    {
        if (isInteracting)
        {
            var t = eventData.NewValue;
            mapRenderer.ZoomLevel = Mathf.Lerp(mapRenderer.MinimumZoomLevel, mapRenderer.MaximumZoomLevel, t);
        }
    }

    public void OnSliderInteractionStart()
    {
        isInteracting = true;
        mapRenderer.GetComponent<MapInteractionController>()?.OnInteractionStarted.Invoke();
    }

    public void OnSliderInteractionEnded()
    {
        isInteracting = false;
        mapRenderer.GetComponent<MapInteractionController>()?.OnInteractionEnded.Invoke();
    }

    public void ResetZoom()
    {
        slider.SliderValue = (defaultZoom - mapRenderer.MinimumZoomLevel) /
                             (mapRenderer.MaximumZoomLevel - mapRenderer.MinimumZoomLevel);
    }
}
