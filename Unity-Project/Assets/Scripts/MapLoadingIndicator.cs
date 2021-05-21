// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MapRenderer))]
public class MapLoadingIndicator : MonoBehaviour
{
    [Header("Indicators")]
    [Tooltip("Animated orbs to show when during the initial scene load")]
    public ProgressIndicatorOrbsRotator sceneLoadingOrbs;

    [Tooltip("GameObject to show when the map is loading data")]
    public GameObject mapLoadingIndicator;

    [Header("Events")]
    public UnityEvent onSceneLoaded;

    private MapRenderer mapRenderer;
    private bool initialLoadComplete;

    private void Awake()
    {
        //Subscribe to an event after each map update.
        mapRenderer = GetComponent<MapRenderer>();
        mapRenderer.AfterUpdate += AfterMapUpdate;
    }

    private void Start()
    {
        initialLoadComplete = false;
        sceneLoadingOrbs.gameObject.SetActive(true);
        sceneLoadingOrbs.OpenAsync();
    }

    private void OnDestroy()
    {
        mapRenderer.AfterUpdate -= AfterMapUpdate;
    }

    private void AfterMapUpdate(object sender, EventArgs e)
    {
        if (!initialLoadComplete && mapRenderer.IsLoaded)
        {
            OnSceneLoadComplete();
        }

        if (initialLoadComplete)
        {
            //Only show the loading message if the map is not fully loaded
            bool showLoading = !mapRenderer.IsLoaded;
            mapLoadingIndicator.SetActive(showLoading);
        }
    }

    private void OnSceneLoadComplete()
    {
        initialLoadComplete = true;
        sceneLoadingOrbs.StopOrbs();
        sceneLoadingOrbs.gameObject.SetActive(false);
        onSceneLoaded?.Invoke();
    }
}