using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SignalRFunctions
{
    public static class SignalRFunctions
    {
        public static string turbineId;
        public static string timeInterval;
        public static string description;
        public static int code;
        public static double windSpeed;
        public static double temperature;
        public static double rotorRPM;
        public static double power = 0.0D;
        public static bool alert;

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "dttelemetry")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("broadcast")]
        public static Task SendMessage(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [SignalR(HubName = "dttelemetry")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            JObject eventGridData = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            if (eventGridEvent.EventType.Contains("telemetry"))
            {
                var data = eventGridData.SelectToken("data");

                var telemetryMessage = new Dictionary<object, object>();
                foreach (JProperty property in data.Children())
                {
                    log.LogInformation(property.Name + " - " + property.Value);
                    telemetryMessage.Add(property.Name, property.Value);
                }
                return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "TelemetryMessage",
                    Arguments = new[] { telemetryMessage }
                });
            }
            else
            {
                try
                {
                    turbineId = eventGridEvent.Subject;
                    
                    var data = eventGridData.SelectToken("data");
                    var patch = data.SelectToken("patch");
                    foreach(JToken token in patch)
                    {
                        if(token["path"].ToString() == "/Alert")
                        {
                            alert = token["value"].ToObject<bool>();
                        }
                    }

                    log.LogInformation($"setting alert to: {alert}");
                    var property = new Dictionary<object, object>
                    {
                        {"TurbineID", turbineId },
                        {"Alert", alert }
                    };
                    return signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "PropertyMessage",
                            Arguments = new[] { property }
                        });
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                    return null;
                }
            }

        }
    }
}