// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Maps.Unity;
using UnityEngine;

/// <summary>
/// Changes the MapRenderer edge color when hovered
/// </summary>
[RequireComponent(typeof(MapRenderer))]
public class MapRendererHover : MonoBehaviour
{
    public Color hoveredColor;
    private Color defaultColor;
    private MapRenderer mapRenderer;
    
    private void Awake()
    {
        mapRenderer = GetComponent<MapRenderer>();
        defaultColor = mapRenderer.MapEdgeColor;
    }

    public void OnHoverStart()
    {
        mapRenderer.MapEdgeColor = hoveredColor;
    }

    public void OnHoverEnd()
    {
        mapRenderer.MapEdgeColor = defaultColor;
    }
}
