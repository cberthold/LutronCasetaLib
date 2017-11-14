using Example;
using Example.Core;
using LutronCaseta.Connectors;
using LutronCaseta.Core.Info;
using LutronCaseta.Core.Options;
using LutronCaseta.Discovery;
using LutronCaseta.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace LutronCaseta
{
    class Program
    {

        static void Main(string[] args)
        {
            SynchronizationContext.SetSynchronizationContext(new AsyncSynchronizationContext());
            // ensure unobserved task exceptions (unawaited async methods returning Task or Task<T>) are handled
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                e.SetObserved();
                var exception = e.Exception;
                ExceptionHandler.Handle(exception);
            };
            Task task = Task.Run(RunMe);
            Console.ReadKey();
        }


        public static async Task RunMe()
        {
            // get the devices on my network
            var devices = await new LutronDiscovery().DiscoverAllLutronDevices();
            // find the first one (there is only one at my house)
            var firstDevice = devices.FirstOrDefault();

            if (firstDevice == null)
            {
                throw new Exception("Where is my bridge pro?");
            }

            IPAddress bridgeAddress = firstDevice.IPAddress;
            var cancelToken = new CancellationToken();

            var extraction = new CertificateExtraction();

            var options = new BridgeSslStreamOptions(bridgeAddress)
            {
                Logging = new ConsoleLogging(),
                LocalCertificateSelectionPolicy = (a1, a2, a3, a4) => extraction.KeyedCertificate,
            };

            using (var connector = new BridgeSslStreamConnector(options, cancelToken))
            {
                var zone1 = new TestZone();
                bool isConnected = await connector.Connect();
                connector.Responses.Subscribe((response) =>
                {
                    var r = response;
                });
                var result = await connector.Ping();
                await Task.Delay(2000);
                var result2 = await connector.GetDevices();
                await Task.Delay(2000);
                connector.SendGetScenes();
                await Task.Delay(2000);
                connector.SendGetZoneStatus(zone1);
                await Task.Delay(2000);
                connector.SetZoneLevel(zone1, 50);
                await Task.Delay(2000);
                connector.TurnOffZone(zone1);
                await Task.Delay(2000);
                connector.TurnOnZone(zone1);

            }

        }

        private class TestZone : IZoneInfo
        {
            public int Id => 1;
        }
    }

}


