// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays a floating message panel to the user
/// </summary>
public class MessageBoxUI : MonoBehaviour
{
    [SerializeField] 
    private string headerText;
    
    [SerializeField] 
    private string bodyText;
    
    [Header("UI Components")]
    [SerializeField]
    private TextMeshPro headerTextMesh;

    [SerializeField] 
    private TextMeshPro bodyTextMesh;

    public Action onDismissed;

    public void ShowMessage(string header, string body)
    {
        headerText = header;
        bodyText = body;
        headerTextMesh.text = headerText;
        bodyTextMesh.text = bodyText;
    }

    public void OnDisable()
    {
        onDismissed?.Invoke();
    }

    private void OnValidate()
    {
        if (bodyTextMesh && headerTextMesh)
        {
            ShowMessage(headerText, bodyText);
        }
    }
}
