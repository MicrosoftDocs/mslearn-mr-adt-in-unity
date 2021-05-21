// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UI Panel which lists all the turbines in the scene.
/// </summary>
public class SiteOverviewUIPanel : MonoBehaviour
{
    public TurbineSiteData siteData;
    public SiteOverviewTurbineButton turbineButtonPrefab;
    public RectTransform contentTransform;
    public WindTurbineUIPanel hoverPanel;
    
    [SerializeField]
    private ProgressController powerOutputBar;
    
    private Dictionary<WindTurbineScriptableObject, SiteOverviewTurbineButton> turbineButtons;

    private void Start()
    {
        ClearContent();
        PopulateUIMenu();
        OnHoverTurbineEnd();
        UpdateTotalPowerOutput();
    }

    private void PopulateUIMenu()
    {
        turbineButtons = new Dictionary<WindTurbineScriptableObject, SiteOverviewTurbineButton>();
        foreach (var turbineData in siteData.turbineData)
        {
            var button = Instantiate(turbineButtonPrefab, contentTransform);
            button.siteOverviewPanel = this;
            button.WindTurbineData = turbineData;
            turbineData.onDataUpdated += UpdateTotalPowerOutput;
            turbineButtons.Add(turbineData, button);
        }
    }

    private void UpdateTotalPowerOutput()
    {
        var averagePower = siteData.turbineData.Select(turbineData => turbineData.windTurbineData.Power).Average();
        powerOutputBar.CurrentValue = averagePower;
    }

    public void OnHoverTurbineButton(WindTurbineScriptableObject turbineData)
    {
        if (hoverPanel)
        {
            hoverPanel.gameObject.SetActive(true);
            hoverPanel.SetTurbineData(turbineData);
        }
    }

    public void OnHoverTurbineEnd()
    {
        if (hoverPanel)
        {
            hoverPanel.gameObject.SetActive(false);
        }
    }

    private void ClearContent()
    {
        var buttons = GetComponentsInChildren<SiteOverviewTurbineButton>();
        foreach (var button in buttons)
        {
            if (Application.isPlaying)
            {
                Destroy(button.gameObject);
            }
            else
            {
                //Make this safe to call outside of playmode
                DestroyImmediate(button.gameObject);
            }
        }
    }
}