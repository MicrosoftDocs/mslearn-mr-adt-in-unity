// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Handles summoning the UI Panels in the Operate Scene
/// </summary>
public class OperateSceneUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField]
    private SiteOverviewUIPanel siteOverviewPanel;

    [SerializeField]
    private WindTurbineUIPanel windTurbinePanel;

    [Header("UI Alert Prefabs")]
    [SerializeField]
    private WindTurbineAlert alertPopupPrefab;

    private WindTurbineAlert alertPopup;

    [SerializeField]
    private MessageBoxUI messageBoxPrefab;

    private MessageBoxUI messageBox;

    [SerializeField]
    private GameObject successPopupPrefab;

    private GameObject successPopup;

    private void Start()
    {
        windTurbinePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the Site Overview panel in the scene
    /// </summary>
    public void ShowSiteOverviewPanel(bool enableFollowMe = true)
    {
        siteOverviewPanel.gameObject.SetActive(true);
        siteOverviewPanel.GetComponent<FollowMeToggle>()?.SetFollowMeBehavior(enableFollowMe);
    }

    /// <summary>
    /// Called when a turbine is selected in the scene or Site Overview Panel
    /// </summary>
    public void OnWindTurbineSelected(WindTurbineScriptableObject turbineData)
    {
        windTurbinePanel.gameObject.SetActive(true);
        windTurbinePanel.SetTurbineData(turbineData);
    }

    /// <summary>
    /// Shows an alert message for a specific turbine
    /// </summary>
    /// <param name="eventData"></param>
    public void ShowAlertMessageForTurbine(ADTTurbineEventData eventData)
    {
        if (alertPopup == null)
        {
            alertPopup = Instantiate(alertPopupPrefab);
        }

        alertPopup.gameObject.SetActive(true);
        alertPopup.ShowAlertMessageForTurbine(eventData);
    }

    public void OnResetCommandSent(WindTurbineScriptableObject turbineData)
    {
        if (messageBox == null)
        {
            messageBox = Instantiate(messageBoxPrefab);
        }

        messageBox.onDismissed += OnMessageBoxDismissed;
        messageBox.gameObject.SetActive(true);
        messageBox.ShowMessage("Reset Command Sent", $"A reset command has successfully been sent to Turbine" +
                                                     $" {turbineData.windTurbineData.TurbineId}");
    }

    private void OnMessageBoxDismissed()
    {
        messageBox.onDismissed -= OnMessageBoxDismissed;
        ShowSuccessMessage();
    }

    /// <summary>
    /// Shows a success message panel
    /// </summary>
    private void ShowSuccessMessage()
    {
        if (successPopup == null)
        {
            successPopup = Instantiate(successPopupPrefab);
        }

        successPopup.gameObject.SetActive(true);
    }
}