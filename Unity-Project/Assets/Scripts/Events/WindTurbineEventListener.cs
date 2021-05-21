// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

public class WindTurbineEventListener : MonoBehaviour
{
    [SerializeField]
    private WindTurbineGameEvent gameEvent;

    [SerializeField]
    public WindTurbineUnityEvent response;

    private void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public void OnEventRaised(WindTurbineScriptableObject turbineData)
    {
        response.Invoke(turbineData);
    }
}