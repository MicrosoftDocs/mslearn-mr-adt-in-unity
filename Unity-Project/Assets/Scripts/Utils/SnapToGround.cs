// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

/// <summary>
/// Raycasts the scene and sets the transform position.
/// When grabbed by a pointer it will allow movement above ground
/// </summary>
public class SnapToGround : MonoBehaviour, IMixedRealityPointerHandler
{
    public LayerMask layerMask;
    public float smoothingTime = 8f;

    public TargetPositionIndicator positionIndicatorPrefab;
    private TargetPositionIndicator positionIndicator;

    private Vector3 RayPosition => transform.position + new Vector3(0, 10, 0);
    private bool isInteracting;
    private Vector3 targetPosition;

    private void Start()
    {
        if (positionIndicatorPrefab != null)
        {
            positionIndicator = Instantiate(positionIndicatorPrefab, transform, true);
            positionIndicator.targetObject = transform;
        }
    }

    private void Update()
    {
        var ray = new Ray(RayPosition, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, 100f, layerMask))
        {
            ShowGroundIndicator(false);
            return;
        }

        ShowGroundIndicator(isInteracting);
        targetPosition = raycastHit.point;

        if (isInteracting)
        {
            //Ensure position is always above ground when interacting
            if (transform.position.y < raycastHit.point.y)
            {
                transform.position = raycastHit.point;
            }

            if (!positionIndicator) return;

            positionIndicator.transform.position = targetPosition;
            positionIndicator.transform.rotation = Quaternion.LookRotation(raycastHit.normal);
            return;
        }

        if (smoothingTime == 0)
        {
            transform.position = targetPosition;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothingTime);
    }

    private void ShowGroundIndicator(bool show)
    {
        if (positionIndicator)
        {
            positionIndicator.gameObject.SetActive(show);
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        isInteracting = true;
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        isInteracting = false;
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }
}