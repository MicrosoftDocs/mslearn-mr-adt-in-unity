// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Handles showing a tooltip when a turbine is hovered in the scene or UI.
/// </summary>
public class WindTurbineTooltipController : MonoBehaviour
{
    public TurbineSiteData siteData;
    public GameObject hoverIndicator;
    public ToolTip toolTip;

    private GameObject hoveredTurbine;

    private void Start()
    {
        hoverIndicator.SetActive(false);
        toolTip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (hoveredTurbine == null)
            return;

        toolTip.gameObject.SetActive(hoveredTurbine.activeInHierarchy);
        toolTip.transform.position = hoveredTurbine.transform.position;
    }

    public void OnHoverStart(WindTurbineScriptableObject turbineData)
    {
        if (siteData.TryGetTurbineGameObject(turbineData, out hoveredTurbine))
        {
            hoverIndicator.SetActive(true);
            hoverIndicator.transform.SetParent(hoveredTurbine.transform);
            hoverIndicator.transform.localPosition = Vector3.zero;
            toolTip.gameObject.SetActive(true);
            toolTip.ToolTipText = turbineData.windTurbineData.TurbineId;
            toolTip.transform.position = hoveredTurbine.transform.position;
        }
    }

    public void OnHoverEnd(WindTurbineScriptableObject turbineData)
    {
        siteData.TryGetTurbineGameObject(turbineData, out var turbine);
        if (turbine == hoveredTurbine)
        {
            hoveredTurbine = null;
            hoverIndicator.SetActive(false);
            toolTip.gameObject.SetActive(false);
        }
    }
}