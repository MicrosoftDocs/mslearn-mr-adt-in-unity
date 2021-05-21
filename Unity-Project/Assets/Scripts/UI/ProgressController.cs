// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Progress Controller to update UI elements based on current value
/// </summary>
public class ProgressController : MonoBehaviour
{
    private const float FillEdgePadding = 0.08f;
    
    public TelemetryRangeData telemetryRangeData;
    public Image imageFill;
    public TextMeshProUGUI textValue;
    public TextMeshProUGUI textUnit;
    
    [SerializeField]
    private double currentValue;
    public double CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            UpdateUI();
        }
    }

    // Update image fill value/color and display text
    private void UpdateUI()
    {
        //Get the current percent 0-1 between the min-max value range
        var percent = (float)((currentValue - telemetryRangeData.minValue) /
                      (telemetryRangeData.maxValue - telemetryRangeData.minValue));   
        
        // Fill amount is based on range from 0(empty)-1(full)
        // Add small padding value to offset image cutout
        float fillValue = Mathf.Lerp(FillEdgePadding, 1 - FillEdgePadding, percent);
        imageFill.fillAmount = fillValue;
        imageFill.color = telemetryRangeData.healthIndicatorGradient.Evaluate(percent);
        
        textValue.text = Math.Round(currentValue, telemetryRangeData.displayRoundingPrecision).ToString("F");
        if (textUnit)
        {
            textUnit.text = telemetryRangeData.units;
        }
    }

    private void OnValidate()
    {
        if (telemetryRangeData && imageFill && textValue)
        {
            UpdateUI();
        }
    }
}