// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Creates a MapPin and links this GameObjects position on a MapRenderer.
/// Allows for interaction with MRTK ObjectManipulator.
/// </summary>
public class DraggableMapPin : MonoBehaviour, IMixedRealityPointerHandler
{
    [Tooltip("Delay in seconds before grabbing the turbine")]
    public float grabTimeWait = 0.2f;

    public TargetPositionIndicator targetIndicatorPrefab;

    public MapPin LinkedMapPin { get; private set; }

    private Vector3 RayPosition => transform.position + new Vector3(0, 10, 0);

    private float returnToPositionSpeed = 0.5f;
    private MapRenderer mapRenderer;
    private MapInteractionController mapController;
    private TargetPositionIndicator targetIndicator;

    private bool isPointerDown;
    private bool isInteracting;
    private float startGrabTime;
    private float elapsedGrabTime;
    private Vector3 targetPosition;
    private IEnumerator currentAnimation;

    private void Awake()
    {
        //Get the MapRenderer and try subscribe to the interaction events
        mapRenderer = GetComponentInParent<MapRenderer>();
        mapController = mapRenderer.GetComponent<MapInteractionController>();
        if (mapController)
        {
            mapController.OnInteractionStarted.AddListener(OnMapInteractionStarted);
        }

        LinkedMapPin = CreateAndLinkToMapPin(this);
        UpdateMapPinLocation();

        targetIndicator = Instantiate(targetIndicatorPrefab, transform, true);
        targetIndicator.targetObject = transform;
    }

    private IEnumerator MoveToTargetPosition(Vector3 finalPosition, float speed)
    {
        float step = (speed / (transform.position - finalPosition).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            transform.position = Vector3.Lerp(transform.position, finalPosition, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = finalPosition;
        currentAnimation = null;
    }

    private void OnDisable()
    {
        isPointerDown = false;
        isInteracting = false;
        ShowGroundIndicator(false);
        GetComponent<ObjectManipulator>()?.ForceEndManipulation();
    }

    private void OnDestroy()
    {
        if (mapController)
        {
            mapController.OnInteractionStarted.RemoveListener(OnMapInteractionStarted);
        }
    }

    public void SetLocation(LatLon latLon)
    {
        LinkedMapPin.Location = latLon;
        transform.position = LinkedMapPin.transform.position;
        transform.localScale = LinkedMapPin.transform.localScale;
    }

    private void Update()
    {
        ShowGroundIndicator(isInteracting);

        if (isPointerDown)
        {
            elapsedGrabTime = Time.time - startGrabTime;
            if (elapsedGrabTime >= grabTimeWait)
            {
                isInteracting = true;
            }
        }

        if (isInteracting)
        {
            Ray ray = new Ray(RayPosition, Vector3.down);
            if (mapRenderer.Raycast(ray, out var mapRaycastHit))
            {
                //Ensure position is always above ground when interacting
                if (transform.position.y < mapRaycastHit.Point.y)
                {
                    transform.position = mapRaycastHit.Point;
                }

                targetPosition = mapRaycastHit.Point;
                targetIndicator.transform.position = targetPosition;
                targetIndicator.transform.rotation = Quaternion.LookRotation(mapRaycastHit.Normal);
            }

            UpdateMapPinLocation();
            return;
        }

        if (currentAnimation != null)
        {
            return;
        }

        SnapTransformToPin();
    }

    private void SnapTransformToPin()
    {
        transform.position = LinkedMapPin.transform.position;
        transform.localScale = LinkedMapPin.transform.localScale;
    }

    /// <summary>
    /// Create a MapPin to link this GameObjects transform to but still allow manipulation
    /// </summary>
    public static MapPin CreateAndLinkToMapPin(DraggableMapPin draggableMapPin)
    {
        MapPin pin = new GameObject($"{draggableMapPin.gameObject.name} - Map Pin").AddComponent<MapPin>();
        pin.transform.parent = draggableMapPin.transform.parent;
        pin.UseRealWorldScale = true;
        pin.ScaleCurve = new AnimationCurve(new Keyframe(1, 5));
        pin.gameObject.AddComponent<MapPinLinkedObject>().linkedObject = draggableMapPin.gameObject;
        return pin;
    }

    private Vector3 UpdateMapPinLocation()
    {
        Vector3 localPos = mapRenderer.transform.InverseTransformPoint(transform.position);
        var coordinate = mapRenderer.TransformLocalPointToMercator(localPos);
        LinkedMapPin.Location = coordinate.ToLatLon();
        return LinkedMapPin.transform.position;
    }

    private void ShowGroundIndicator(bool show)
    {
        if (targetIndicator)
        {
            targetIndicator.gameObject.SetActive(show);
        }
    }

    //When the user starts scrolling the map
    private void OnMapInteractionStarted()
    {
        //Cancel the current animation and snap to position
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            SnapTransformToPin();
        }

        currentAnimation = null;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        isPointerDown = true;
        startGrabTime = Time.time;
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        isPointerDown = false;
        isInteracting = false;
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = MoveToTargetPosition(LinkedMapPin.transform.position, returnToPositionSpeed);
        StartCoroutine(currentAnimation);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }
}