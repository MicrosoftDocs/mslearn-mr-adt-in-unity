// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Listens to an ADT Turbine Event and defines the response to it
/// </summary>
public class ADTTurbineEventListener : MonoBehaviour
{
    /// <summary>
    /// Event to listen to
    /// </summary>
    [SerializeField]
    private ADTTurbineGameEvent gameEvent;

    /// <summary>
    /// Action to take in response to the event
    /// </summary>
    [SerializeField]
    public ADTTurbineUnityEvent response;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    /// <summary>
    /// Invokes the response upon receiving the event
    /// </summary>
    /// <param name="eventData">Data about the event</param>
    public void OnEventRaised(ADTTurbineEventData eventData)
    {
        response.Invoke(eventData);
    }
}
