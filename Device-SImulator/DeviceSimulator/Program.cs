using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DeviceSimulator
{
    public class Program

    {
        public static async Task Main(string[] args)
        {
            await CreateDeviceIdentities();
            await SimulateDeviceToSendD2cAndReceiveD2c();
        }

        /// <summary>
        /// Creates devices or gets devices from IoT hub
        /// </summary>
        /// <returns></returns>
        public static async Task CreateDeviceIdentities()
        {
            List<string> deviceNames = DeviceIdLoader.GetDeviceIds();
            await AzureIoTHub.CreateDeviceIdentitiesAsyc(deviceNames);
            foreach(string deviceName in deviceNames)
            {
                Console.WriteLine($"Device with name '{deviceName}' was created/retrieved successfully");
            }
        }

        private static async Task SimulateDeviceToSendD2cAndReceiveD2c()
        {
            var tokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
                Console.WriteLine("Exiting...");
            };
            Console.WriteLine("Press CTRL+C to exit");
            Console.WriteLine("Press any key to begin the simulation");
            Console.WriteLine("During the simulation press Spacebar to send an alert and to clear the alert if one has already been sent)");
            Console.ReadKey();
            await Task.WhenAll(
                AzureIoTHub.SendDeviceToCloudMessageAsync(tokenSource.Token),
                AzureIoTHub.ReceiveMessagesFromDeviceAsync(tokenSource.Token),
                StartInputLoop(tokenSource.Token)
            );

            tokenSource.Dispose();
        }

        private static async Task StartInputLoop(CancellationToken cancelToken)
        {
            do
            {
                var keyEntered = Console.ReadKey().Key;
                if(keyEntered == ConsoleKey.Spacebar)
                {
                    await AzureIoTHub.SendAlert();
                }
                Console.WriteLine();
            } while (!cancelToken.IsCancellationRequested);
        }
    }
}
