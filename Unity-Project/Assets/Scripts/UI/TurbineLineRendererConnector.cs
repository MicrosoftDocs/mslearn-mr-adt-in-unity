// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Connects a LineRenderer to a target wind turbine in the scene
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class TurbineLineRendererConnector : MonoBehaviour
{
    public TurbineSiteData siteData;
    public Transform[] anchorPoints = new Transform[0];
    private Vector3[] anchorPositions;
    private Transform target;
    private LineRenderer lineRenderer;
    private Transform TargetAnchor => anchorPoints[anchorPoints.Length - 1];

    /// <summary>
    /// Set the target wind turbine in the scene from its data object
    /// </summary>
    public void SetTarget(WindTurbineScriptableObject turbineData)
    {
        if (siteData.TryGetTurbineGameObject(turbineData, out var go))
        {
            //Target the rotor of the turbine
            target = go.GetComponentInChildren<RotateRPM>().transform;
        }
    }

    private void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = anchorPoints.Length;
        anchorPositions = new Vector3[anchorPoints.Length];
    }

    private void Update()
    {
        if (!target || !lineRenderer)
            return;

        TargetAnchor.transform.position = target.position;
        SetLinePositions();
    }

    private void SetLinePositions()
    {
        lineRenderer.positionCount = target.gameObject.activeInHierarchy
            ? lineRenderer.positionCount = anchorPoints.Length
            : anchorPoints.Length - 1;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (anchorPoints[i] != null)
            {
                anchorPositions[i] = anchorPoints[i].transform.position;
            }
        }

        lineRenderer.SetPositions(anchorPositions);
    }
}