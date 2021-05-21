// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Updates a LineRenderer to draw between two transforms
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TargetPositionIndicator : MonoBehaviour
{
    public Transform targetObject;

    private LineRenderer lineRenderer;
    private Vector3[] linePositions = new Vector3[2];

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        if (targetObject == null)
        {
            return;
        }
        
        linePositions[0] = targetObject.transform.position;
        linePositions[1] = transform.position;
        lineRenderer.SetPositions(linePositions);
    }
}
