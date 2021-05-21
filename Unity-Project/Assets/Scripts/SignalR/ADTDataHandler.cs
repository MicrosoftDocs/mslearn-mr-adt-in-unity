// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Unity;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ADTDataHandler : MonoBehaviour
{
    private SignalRService rService;

    public string url = "";
    public TurbineSiteData turbineSiteData;
    public WindTurbineGameEvent TurbinePropertyMessageReceived;

    private void Start()
    {
        this.RunSafeVoid(CreateServiceAsync);
    }

    private void OnDestroy()
    {
        if (rService != null)
        {
            rService.OnConnected -= HandleConnected;
            rService.OnTelemetryMessage -= HandleTelemetryMessage;
            rService.OnDisconnected -= HandleDisconnected;
            rService.OnPropertyMessage -= HandlePropertyMessage;
        }
    }

    /// <summary>
    /// Received a message from SignalR. Note, this message is received on a background thread.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    private void HandleTelemetryMessage(TelemetryMessage message)
    {
        // Finally update Unity GameObjects, but this must be done on the Unity Main thread.
        UnityDispatcher.InvokeOnAppThread(() =>
        {
            foreach (WindTurbineScriptableObject turbine in turbineSiteData.turbineData)
            {
                if (turbine.windTurbineData.TurbineId == message.TurbineID)
                {
                    turbine.UpdateData(CreateNewWindTurbineData(message));
                    return;
                }
            }
        });
    }

    /// <summary>
    /// Construct the WindTurbine Data received from SignalR
    /// </summary>
    /// <param name="message">Telemetry data</param>
    /// <returns>Data values of wind turbine</returns>
    private WindTurbineData CreateNewWindTurbineData(TelemetryMessage message)
    {
        WindTurbineData data = new WindTurbineData
        {
            TurbineId = message.TurbineID,
            AmbientTemperature = message.Ambient,
            EventCode = message.Code,
            EventDescription = message.Description,
            Power = message.Power,
            RotorSpeed = message.Rotor,
            TimeInterval = message.TimeInterval,
            WindSpeed = message.WindSpeed,
        };

        return data;
    }

    /// <summary>
    /// Received a Property message from SignalR. Note, this message is received on a background thread.
    /// </summary>
    /// <param name="message">
    /// The message
    /// </param>
    private void HandlePropertyMessage(PropertyMessage message)
    {
        UnityDispatcher.InvokeOnAppThread(() =>
        {
            var matchingTurbines = turbineSiteData.windTurbines.Where(t => t.Key.windTurbineData.TurbineId == message.TurbineID);
            if (!matchingTurbines.Any())
            {
                Debug.LogWarning($"Turbine {message.TurbineID} was not found in the Site Data.");
                return;
            }
            var turbineScriptableObject = matchingTurbines.First().Key;
            turbineScriptableObject.windTurbineMetaData.Alert = message.Alert;
            TurbinePropertyMessageReceived.Raise(turbineScriptableObject);
        });
    }

    private Task CreateServiceAsync()
    {
        rService = new SignalRService();
        rService.OnConnected += HandleConnected;
        rService.OnDisconnected += HandleDisconnected;
        rService.OnTelemetryMessage += HandleTelemetryMessage;
        rService.OnPropertyMessage += HandlePropertyMessage;

        return rService.StartAsync(url);
    }

    private void HandleConnected(string obj)
    {
        Debug.Log("Connected");
    }

    private void HandleDisconnected()
    {
        Debug.Log("Disconnected");
    }
}