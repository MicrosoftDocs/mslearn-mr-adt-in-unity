using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public class PropUpdater
    {
        private string accessToken;
        private readonly Uri adtInstanceUri;
        private readonly string azureLoginUrl = "https://login.microsoftonline.com/{0}/oauth2/token"; //{0}: tenantId

        private string clientId = "";
        private string clientSecret = "";
        private string tenantId = "";

        HttpClient httpClient = null;
        private readonly HttpMethod HTTPMETHOD_PATCH = new HttpMethod("PATCH");

        public PropUpdater(string adtInstanceUrl)
        {
            this.adtInstanceUri = new Uri(adtInstanceUrl);
        }

        public async Task Authenticate()
        {
            if (httpClient == null)
                httpClient = new HttpClient();

            accessToken = await GetAuthToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task<string> GetAuthToken()
        {
            var authTokenUrlWithTenantId = string.Format(azureLoginUrl, tenantId);
            var authTokenUri = new Uri(authTokenUrlWithTenantId);
            var requestParameters = new Dictionary<string, string>();
            requestParameters.Add("client_id", clientId);
            requestParameters.Add("client_secret", clientSecret);
            requestParameters.Add("grant_type", "client_credentials");
            requestParameters.Add("resource", "https://digitaltwins.azure.net");

            var tokenRequestMessage = new HttpRequestMessage(HttpMethod.Get, authTokenUri)
            { Content = new FormUrlEncodedContent(requestParameters) };

            var authResponse = await httpClient.SendAsync(tokenRequestMessage);
            var authResponseContent = await authResponse.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<JObject>(authResponseContent);
            var authToken = responseJson["access_token"].ToString();
            Console.WriteLine($"Auth Token: [{authToken}]");

            return authToken;
        }

        public async Task<JObject> GetTwinData(string twinId)
        {
            await Authenticate();
            var getTwinDataUrl = new Uri(adtInstanceUri, $"digitaltwins/{twinId}/?api-version=2020-10-31");
            var twinResponse = await httpClient.GetAsync(getTwinDataUrl);
            var twinContent = await twinResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Twin Data: [{twinContent}]");

            return JsonConvert.DeserializeObject<JObject>(twinContent);
        }

        public async Task<HttpStatusCode> UpdateBoolProperty(string twinId, string propertyName, bool newValue)
        {
            await Authenticate();
            Console.WriteLine($"Updating {twinId} alert status to {newValue}");
            var newValueString = newValue.ToString().ToLower();
            var updateTwinPropertyUrl = new Uri(adtInstanceUri, $"digitaltwins/{twinId}/?api-version=2020-10-31");
            var updateRequest = new HttpRequestMessage(HTTPMETHOD_PATCH, updateTwinPropertyUrl)
            {
                //The content for this request is a JSON patch.
                //It will be used to replace the value of only a specific property in a larger JSON object which represents the entire turbine. 
                //It is composed of "op" (operation) with a value of "Replace", 
                //"path" which indicates where the property to be changes is within the structure of the JSON object and finally, 
                //"value" which is the desired value to set for the property.
                Content = new StringContent($"[{{\"op\": \"replace\", \"path\": \"/Alert\",\"value\": {newValueString} }}]", Encoding.UTF8, "application/json")
            };
            var twinResponse = await httpClient.SendAsync(updateRequest);
            Console.WriteLine($"Update Response Status Code: [{(int)twinResponse.StatusCode}]");

            return twinResponse.StatusCode;
        }
    }
}
