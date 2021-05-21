// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using BladeMR.Events;
using System;
using UnityEngine;

namespace BladeMR.ADT
{
    /// <summary>
    /// Gets and sets the Alert property of the Wind Turbine on the Azure Digital Twins (ADT) service.
    /// </summary>
    [Serializable]
    public class ADTTurbineAlertController : MonoBehaviour
    {
        /// <summary>
        /// Communicates with the Azure Digital Twins service using its Rest API.
        /// </summary>
        private ADTRestClient adtClient;

        #region Inspector Parameters

        [SerializeField]
        private ADTRestAPICredentialsScriptableObject adtConnectionInfo;

        /// <summary>
        /// The Scriptable Object containing the data of the turbine on which the operations will be performed.
        /// </summary>
        [Header("Turbine")]
        [SerializeField]
        private WindTurbineScriptableObject Turbine;

        [SerializeField]
        private AudioSource alertAudioSource;

        /// <summary>
        /// The ID of the turbine on which the operations will be performed.
        /// </summary>
        private string TurbineId => Turbine.windTurbineData.TurbineId;

        #endregion Inspector Parameters

        #region Events
        /// <summary>
        /// Event used to signal that information has been retrieved from ADT though the Rest API.
        /// </summary>
        [Header("Events")]
        [SerializeField]
        private WindTurbineGameEvent OnADTReceived;

        [SerializeField]
        private ADTAuthenticationGameEvent OnADTAuthenticated;

        [SerializeField]
        private ADTAuthenticationGameEvent OnADTOnADTAuthenticationFailed;
        #endregion Events

        #region Unity Callbacks

        private void Start()
        {
            // Initialize the ADT Rest Client with the service principal credentials
            adtClient = new ADTRestClient(adtConnectionInfo.adtInstanceUrl, adtConnectionInfo.clientId, adtConnectionInfo.clientSecret, adtConnectionInfo.tenantId)
            {
                OnADTReceivedTwinData = OnADTReceived,
                OnADTRestAuthenticate = OnADTAuthenticated,
                OnADTRestAuthenticationFailed = OnADTOnADTAuthenticationFailed
            };
            if(alertAudioSource == null)
            {
                alertAudioSource = GetComponent<AudioSource>();
            }
        }

        public void PlayAlertAudio(WindTurbineScriptableObject turbine)
        {
            if(turbine.windTurbineMetaData.Alert)
            {
                alertAudioSource.PlayOneShot(alertAudioSource.clip);
            }
        }

        /// <summary>
        /// Sets the Alert property of the wind turbine's digital twin to true
        /// </summary>
        public async void SetAlert() => await adtClient.SetAlertOnTwin(TurbineId);

        /// <summary>
        /// Sets the Alert property of the wind turbine's digital twin to false
        /// </summary>
        public async void ClearAlert() => await adtClient.ClearAlertOnTwin(TurbineId);

        /// <summary>
        /// Retrieves the wind turbine's digital twin data from ADT through the Rest API.
        /// </summary>
        public async void GetData() => await adtClient.GetWindTurbineTwinData(TurbineId);

        #endregion Unity Callbacks
    }
}