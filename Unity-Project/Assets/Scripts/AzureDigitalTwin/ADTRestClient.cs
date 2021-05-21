// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using UnityEngine;

namespace BladeMR.ADT
{
    using Assets.Scripts.AzureDigitalTwin;
    using Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Consumes a small subset of the Azure Digital Twins' Rest API.
    /// </summary>
    [Serializable]
    public class ADTRestClient
    {
        #region Public Events

        /// <summary>
        /// Event invoked when authentication with the ADT Rest API succeeds.
        /// The authentication token is passed in.
        /// </summary>
        public ADTAuthenticationGameEvent OnADTRestAuthenticate;

        /// <summary>
        /// Event invoked when authentication with the ADT Rest API fails.
        /// The error mesage is passed in.
        /// </summary>
        public ADTAuthenticationGameEvent OnADTRestAuthenticationFailed;

        /// <summary>
        /// Event invoked when digital twin data is received from ADT.
        /// </summary>
        public WindTurbineGameEvent OnADTReceivedTwinData;

        /// <summary>
        /// Event invoked when a digital twin property is updated on ADT.
        /// </summary>
        public WindTurbineGameEvent OnUpdatedADTTwinProperty;

        #endregion Public Events

        #region Constructor and Destructor

        /// <summary>
        /// ADTRestClient constructor.
        /// </summary>
        /// <param name="adtInstanceUrl">URL of the Azure Digital Twins instance.</param>
        /// <param name="clientId">Client ID of the service principal usde to authenticate with ADT</param>
        /// <param name="clientSecret">Client Secret of the service principal usde to authenticate with ADT</param>
        /// <param name="tenantId">Tenant ID of the service principal usde to authenticate with ADT</param>
        public ADTRestClient(string adtInstanceUrl, string clientId, string clientSecret, string tenantId)
        {
            httpClient = new HttpClient();
            adtInstanceUri = new Uri(adtInstanceUrl);
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenantId = tenantId;
        }

        /// <summary>
        /// ADTRestClient destructor.
        /// </summary>
        ~ADTRestClient()
        {
            httpClient.Dispose();
        }

        #endregion Constructor and Destructor

        #region Private Members

        private readonly Uri adtInstanceUri;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string tenantId;
        private readonly HttpClient httpClient = null;
        private string accessToken;
        private readonly HttpMethod HTTPMETHOD_PATCH = new HttpMethod("PATCH");

        #endregion Private Members

        #region Fixed Azure Parameters
        private string authenticationUrl => string.Format(Constants.Authentication.AUTHENTICATION_URL_TEMPLATE, tenantId);

        #endregion Fixed Azure Parameters

        #region App Logic

        /// <summary>
        /// Sets the Alert property of the Digital Twin to true
        /// </summary>
        /// <param name="twinId">ID of the digital twin to update.</param>
        /// <returns>The status code of the Rest request.</returns>
        public async Task<HttpStatusCode> SetAlertOnTwin(string twinId)
        {
            return await UpdateBoolProperty(twinId, Constants.WindTurbine.ADT_TURBINE_ALERT_PROPERTY, true);
        }

        /// <summary>
        /// Sets the Alert property of the Digital Twin to false
        /// </summary>
        /// <param name="twinId">ID of the digital twin to update.</param>
        /// <returns>The status code of the Rest request.</returns>
        public async Task<HttpStatusCode> ClearAlertOnTwin(string twinId)
        {
            return await UpdateBoolProperty(twinId, Constants.WindTurbine.ADT_TURBINE_ALERT_PROPERTY, false);
        }

        #endregion App Logic

        #region Azure Digital Twin Authentication

        /// <summary>
        /// Authenticates the HTTP client with Azure.
        /// </summary>
        private async Task Authenticate()
        {
            try
            {
                accessToken = await GetAuthToken();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Authentication.AUTHENTICATION_SCHEME, accessToken);

                OnADTRestAuthenticate?.Raise(accessToken);
            }
            catch (Exception ex)
            {
                OnADTRestAuthenticationFailed?.Raise(ex.Message);
            }
        }

        /// <summary>
        /// Gets an authentication token from Azure using a Rest call.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAuthToken()
        {
            HttpRequestMessage authRequest = BuildAuthenticationRequest();
            HttpResponseMessage authResponse = await httpClient.SendAsync(authRequest);
            string authResponseContent = await authResponse.Content.ReadAsStringAsync();
            JObject responseJson = JsonConvert.DeserializeObject<JObject>(authResponseContent);
            string authToken = responseJson[Constants.Authentication.ACCESS_TOKEN_KEY].ToString();

            Debug.Log($"Auth Token: [{authToken}]");

            return authToken;
        }

        /// <summary>
        /// Helper method to create the rest request for authenticating with Azure.
        /// </summary>
        /// <returns>The request that needs to be sent for authenticating with Azure.</returns>
        private HttpRequestMessage BuildAuthenticationRequest()
        {
            var requestParameters = new Dictionary<string, string>
            {
                { Constants.Authentication.CLIENT_ID_KEY, clientId },
                { Constants.Authentication.CLIENT_SECRET_KEY, clientSecret },
                { Constants.Authentication.GRANT_TYPE_KEY, Constants.Authentication.GRANT_TYPE_VALUE },
                { Constants.Authentication.RESOURCE_KEY, Constants.Authentication.ADT_RESOURCETYPE_URI }
            };

            var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, authenticationUrl)
            { Content = new FormUrlEncodedContent(requestParameters) };
            return tokenRequestMessage;
        }

        #endregion Azure Digital Twin Authentication

        #region Azure Digital Twin Requests

        /// <summary>
        /// Retrieves a wind turbine's properties from the ADT service.
        /// </summary>
        /// <param name="twinId">The ID of the turbine whose data is to be retrieved.</param>
        /// <returns>The properties of the turbine as stored in ADT.</returns>
        public async Task<WindTurbineMetaData> GetWindTurbineTwinData(string twinId)
        {
            await Authenticate();

            string twinEndpoint = GetTwinEndpoint(twinId);
            HttpResponseMessage twinResponse = await httpClient.GetAsync(twinEndpoint);
            string twinContent = await twinResponse.Content.ReadAsStringAsync();

            var turbineData = JsonConvert.DeserializeObject<WindTurbineMetaData>(twinContent);
            WindTurbineScriptableObject turbine = new WindTurbineScriptableObject();
            turbine.windTurbineMetaData = turbineData;
            OnADTReceivedTwinData.Raise(turbine);

            Debug.Log($"Twin Data: [{twinContent}]");
            return turbineData;
        }

        /// <summary>
        /// Updates the value of a boolean property on an Azure Digital Twin.
        /// </summary>
        /// <param name="twinId">ID of the digital twin whose property will be updated</param>
        /// <param name="propertyName">Name of the property to be updated</param>
        /// <param name="newValue">Value to be assigned to the property</param>
        /// <returns>Status Code of the HTTP response</returns>
        private async Task<HttpStatusCode> UpdateBoolProperty(string twinId, string propertyName, bool newValue)
        {
            await Authenticate();

            string newValueAsString = newValue.ToString().ToLower();
            string twinEndpoint = GetTwinEndpoint(twinId);
            HttpRequestMessage updateRequest = new HttpRequestMessage(HTTPMETHOD_PATCH, twinEndpoint)
            {
                Content = BuildPatchMessageBody(propertyName, newValueAsString)
            };

            HttpResponseMessage twinResponse = await httpClient.SendAsync(updateRequest);

            Debug.Log($"Update Response Status Code: [{(int)twinResponse.StatusCode}]");

            if (twinResponse.StatusCode == HttpStatusCode.NoContent)
                Debug.Log(Constants.ResponseMessages.UPDATE_SUCCESS);
            else
                Debug.Log(Constants.ResponseMessages.UPDATE_FAILURE);

            return twinResponse.StatusCode;
        }

        #endregion Azure Digital Twin Requests

        #region Azure Digital Twin Request Helpers

        /// <summary>
        /// Builds the URL used to interact with a specific Azure Digital Twin
        /// </summary>
        /// <param name="twinId">ID of the twin to interact with</param>
        /// <returns>The URL to be used in Rest calls to interact with the twin</returns>
        private string GetTwinEndpoint(string twinId) => string.Format(Constants.Requests.DIGITAL_TWIN_URL_TEMPLATE, adtInstanceUri.AbsoluteUri, twinId);

        /// <summary>
        /// Creates the body of a Patch request used to update the a property on an Azure Digital Twin
        /// </summary>
        /// <param name="propertyName">Name of the property to be updated</param>
        /// <param name="newValueAsString">Value to be assigned to the property</param>
        /// <returns>Body of the request to be send when updating a property on ADT</returns>
        private StringContent BuildPatchMessageBody(string propertyName, string newValueAsString)
        {
            return new StringContent(string.Format(Constants.Requests.PATCH_MESSAGE_BODY_TEMPLATE, propertyName, newValueAsString),Encoding.UTF8, Constants.Requests.JSON_CONTENT_TYPE);
        }

        #endregion Azure Digital Twin Request Helpers
    }
}