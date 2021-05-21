using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator
{
    public static class AzureIoTHub
    {
        /// <summary>
        /// Please replace with correct connection string value
        /// The connection string could be got from Azure IoT Hub -> Shared access policies -> iothubowner -> Connection String:
        /// </summary>
        private const string iotHubConnectionString = "";
        private const string adtInstanceUrl = "";
        private const string alertTurbineId = "T102";
        private const string alertVariableName = "Alert";
        private const string alertDescription = "Light icing (rotor bl. ice sensor)";
        private const double alertTemp = -6.0D;
        private const double alertPower = 200.0D;
        private const double alertWindSpeed = 7.0D;
        private const double alertRotorSpeed = 1.4D;
        private const double alertTempVariance = 1.0D;
        private const double alertPowerVariance = 45D;
        private const double alertWindSpeedVariance = 0.40D;
        private const double alertRotorVariance = .10D;
        private const int alertCode = 400;
        private static List<string> deviceConnectionStrings;
        private static bool alertSent = false;
        private static int alertIndex;
        
        public static async Task<List<Device>> CreateDeviceIdentitiesAsyc(List<string> deviceIds)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            List<Device> devices = new List<Device>();
            deviceConnectionStrings = new List<string>();
            for(int i = 0; i < deviceIds.Count; i++)
            {
                var device = new Device(deviceIds[i]);
                device = await CreateOrGetDevice(registryManager, device);
                devices.Add(device);
                if(device.Id == alertTurbineId)
                {
                    alertIndex = i;
                }
                deviceConnectionStrings.Add(CreateConnectionString(device));
            }
            return devices;
        }

        private static string CreateConnectionString(Device device)
        {
            string connectionString = string.Format("{0};DeviceId={1};SharedAccessKey={2}", iotHubConnectionString.Split(';')[0], device.Id.ToString(), device.Authentication.SymmetricKey.PrimaryKey.ToString());
            return connectionString;
        }

        private static async Task<Device> CreateOrGetDevice(RegistryManager registryManager, Device device)
        {
            try
            {
                Device createdDevice = await registryManager.AddDeviceAsync(device);
                Console.WriteLine("Adding device " + device.Id);
                return createdDevice;
            }
            catch(DeviceAlreadyExistsException)
            {
                Console.WriteLine("Retrieved device " + device.Id);
                return await registryManager.GetDeviceAsync(device.Id);
            }
        }

        public static async Task SendDeviceToCloudMessageAsync(CancellationToken cancelToken)
        {
            List<DeviceClient> deviceClients = new List<DeviceClient>();
            foreach (string deviceConnectionString in deviceConnectionStrings)
            {
                //use connection string to create a device client
                deviceClients.Add(DeviceClient.CreateFromConnectionString(deviceConnectionString));
            }
            
            List<TelemetryData> data = Telemetry.GetDataLines();
            int dataIterator = 0;
            while (!cancelToken.IsCancellationRequested)
            {
                for (int i = 0; i < deviceClients.Count; i++)
                {
                    Microsoft.Azure.Devices.Client.Message message = new Microsoft.Azure.Devices.Client.Message();
                    if (alertSent && data[i + dataIterator].turbineId == alertTurbineId)
                    {
                        // This is sending a specified Alert message
                        message = ConstructTelemetryDataPoint(data[i + dataIterator], isAlert: true);
                        PropUpdater propUpdater = new PropUpdater(adtInstanceUrl);
                        var twinData = await propUpdater.GetTwinData(alertTurbineId);
                        if (twinData.Value<bool>(alertVariableName) == false)
                        {
                            alertSent = false;
                        }
                    }
                    else
                    {
                        //Basic telemetry message without alert
                        message = ConstructTelemetryDataPoint(data[i + dataIterator], isAlert: false);
                    }
                    await deviceClients[i].SendEventAsync(message);
                }
                if (dataIterator < data.Count - deviceClients.Count)
                {
                    dataIterator += deviceClients.Count;
                }
                else
                {
                    Console.WriteLine("Press any key to restart the data sequence");
                    Console.ReadKey();
                    dataIterator = 0;
                }
                await Task.Delay(5000);
                //Keep this value above 1000 to keep a safe buffer above the ADT service limits
                //See https://aka.ms/adt-limits for more info
            }
        }

        private static Microsoft.Azure.Devices.Client.Message ConstructTelemetryDataPoint(TelemetryData data, bool isAlert)
        {
            Random rand = new Random();
            TelemetryData telData = new TelemetryData(data);
            if(isAlert)
            {
                telData.eventCodeDescription = alertDescription;
                telData.eventCode = alertCode;
                telData.windSpeed = alertWindSpeed + (alertWindSpeedVariance * rand.NextDouble());
                telData.temperature = alertTemp + (alertTempVariance * rand.NextDouble());
                telData.rotorSpeed = alertRotorSpeed + (alertRotorVariance * rand.NextDouble());
                telData.power = alertPower + (alertPowerVariance * rand.NextDouble());
            }

            var payload = new
            {
                TurbineID = telData.turbineId,
                TimeInterval = telData.timeInterval,
                Description = telData.eventCodeDescription,
                Code = telData.eventCode,
                WindSpeed = telData.windSpeed,
                Ambient = telData.temperature,
                Rotor = telData.rotorSpeed,
                Power = telData.power
            };
            var messageString = JsonSerializer.Serialize(payload);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");
            return new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };
        }

        public static async Task SendAlert()
        {
            try
            {
                var payload = new
                {
                    TurbineID = alertTurbineId,
                    Alert = !alertSent
                };
                var messageString = JsonSerializer.Serialize(payload);
                var client = DeviceClient.CreateFromConnectionString(deviceConnectionStrings[alertIndex]);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };
                
                await client.SendEventAsync(message);
                alertSent = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task ReceiveMessagesFromDeviceAsync(CancellationToken cancelToken)
        {
            try
            {
                string eventHubConnectionString = await IotHubConnection.GetEventHubsConnectionStringAsync(iotHubConnectionString);
                await using var consumerClient = new EventHubConsumerClient(
                    EventHubConsumerClient.DefaultConsumerGroupName,
                    eventHubConnectionString);

                await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync(cancelToken))
                {
                    if (partitionEvent.Data == null) continue;

                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Message received. Partition: {partitionEvent.Partition.PartitionId} Data: '{data}'");
                }
            }
            catch (TaskCanceledException) { } // do nothing
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading event: {ex}");
            }
        }
    }
}
