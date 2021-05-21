// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AzureDigitalTwin
{
    public static class Constants
    {
        public static class Authentication
        { 
            public const string API_VERSION = "2020-10-31";
            public const string AUTHENTICATION_URL_TEMPLATE = "https://login.microsoftonline.com/{0}/oauth2/token";
            public const string ADT_RESOURCETYPE_URI = "https://digitaltwins.azure.net";
            public const string CLIENT_ID_KEY = "client_id";
            public const string CLIENT_SECRET_KEY = "client_secret";
            public const string GRANT_TYPE_KEY = "grant_type";
            public const string GRANT_TYPE_VALUE = "client_credentials";
            public const string RESOURCE_KEY = "resource";
            public const string ACCESS_TOKEN_KEY = "access_token";
            public const string AUTHENTICATION_SCHEME = "Bearer";
        }

        public static class Requests
        { 
            public const string DIGITAL_TWIN_URL_TEMPLATE = "{0}/digitaltwins/{1}/?api-version=" + Authentication.API_VERSION; // 0: ADT instance URL, 1: Tenant ID
            public const string PATCH_MESSAGE_BODY_TEMPLATE = "[{{\"op\": \"replace\", \"path\": \"/{0}\",\"value\": {1} }}]"; // 0:propertyName, 1: newValueAsString
            public const string JSON_CONTENT_TYPE = "application/json";
        }

        public static class ResponseMessages
        {
            public const string UPDATE_SUCCESS = "Update succeeded";
            public const string UPDATE_FAILURE = "Update failed";
        }

        public static class WindTurbine
        {
            public const string ADT_TURBINE_ALERT_PROPERTY = "Alert";
        }
    }
}
