// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

/// <summary>
/// Switches the material on a mesh when hovered by a MRTK pointer
/// </summary>
public class HoverMaterialSwitcher : MonoBehaviour, IMixedRealityFocusHandler
{
    public MeshRenderer meshRenderer;

    public Material hoverMaterial;

    private Material defaultMaterial;

    private void Awake()
    {
        defaultMaterial = meshRenderer.material;
    }

    private void OnDisable()
    {
        OnFocusExit(null);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        meshRenderer.material = hoverMaterial;
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        meshRenderer.material = defaultMaterial;
    }
}