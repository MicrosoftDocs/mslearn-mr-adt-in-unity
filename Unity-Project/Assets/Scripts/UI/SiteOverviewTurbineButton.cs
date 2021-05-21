// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

public class SiteOverviewTurbineButton : MonoBehaviour, IMixedRealityFocusHandler
{
    public SiteOverviewUIPanel siteOverviewPanel;

    [Header("UI Components")]
    [SerializeField]
    private TextMeshProUGUI turbineNameLabel;

    [SerializeField]
    private ProgressController progressController;

    [SerializeField]
    private GameObject warningIndicator;

    private WindTurbineScriptableObject windTurbineData;

    public WindTurbineScriptableObject WindTurbineData
    {
        get => windTurbineData;
        set
        {
            windTurbineData = value;
            windTurbineData.onDataUpdated += OnWindTurbineDataChanged;
            OnWindTurbineDataChanged();
        }
    }

    [Header("Events")]
    public WindTurbineGameEvent focusOnTurbineEvent;

    public WindTurbineGameEvent onHoverStart;
    public WindTurbineGameEvent onHoverEnd;

    private void OnValidate()
    {
        if (windTurbineData)
        {
            OnWindTurbineDataChanged();
        }
    }

    private void ShowWarningIndicator(bool show)
    {
        warningIndicator.gameObject.SetActive(show);
    }

    private void OnWindTurbineDataChanged()
    {
        turbineNameLabel.text = $"Turbine {windTurbineData.windTurbineData.TurbineId}";
        progressController.CurrentValue = windTurbineData.windTurbineData.Power;
        ShowWarningIndicator(windTurbineData.windTurbineMetaData.Alert);
    }

    public void OnClicked()
    {
        focusOnTurbineEvent.Raise(windTurbineData);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        siteOverviewPanel.OnHoverTurbineButton(WindTurbineData);
        onHoverStart.Raise(WindTurbineData);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        siteOverviewPanel.OnHoverTurbineEnd();
        onHoverEnd.Raise(WindTurbineData);
    }
}