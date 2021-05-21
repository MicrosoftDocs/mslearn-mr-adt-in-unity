// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

/// <summary>
/// Shows the position/direction of a MapPin currently out of view
/// </summary>
public class MapPinIndicator : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
{
    private static readonly int ColorID = Shader.PropertyToID("_Color");
    
    [SerializeField]
    private Color defaultColor;
    
    [SerializeField]
    private Color alertColor;

    [SerializeField] 
    private WindTurbineGameEvent onTurbineSelected;
    
    private WindTurbineScriptableObject turbineData;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock matPropertyBlock;
    
    public bool HasAlert { get; private set; }

    private Vector3 defaultScale => Vector3.one;
    private Vector3 alertScale => Vector3.one * 2f;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        matPropertyBlock = new MaterialPropertyBlock();
    }

    public void Setup(WindTurbineScriptableObject data)
    {
        turbineData = data;
        turbineData.onDataUpdated += OnTurbineDataUpdated;
        OnTurbineDataUpdated();
    }

    private void OnTurbineDataUpdated()
    {
        HasAlert = turbineData.windTurbineMetaData.Alert;
        var color = HasAlert ? alertColor : defaultColor;
        var scale = HasAlert ? alertScale : defaultScale;
        matPropertyBlock.SetColor(ColorID, color);
        meshRenderer.SetPropertyBlock(matPropertyBlock);
        transform.localScale = scale;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        onTurbineSelected.Raise(turbineData);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        var baseScale = HasAlert ? alertScale : defaultScale;
        transform.localScale = baseScale * 1.5f;
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        transform.localScale = HasAlert ? alertScale : defaultScale;
    }
    
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
}
