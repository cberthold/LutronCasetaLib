using Example;
using Example.Core;
using LutronCaseta.Connectors;
using LutronCaseta.Discovery;
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
            var devices = await LutronDiscovery.DiscoverAllLutronDevices();
            // find the first one (there is only one at my house)
            var firstDevice = devices.FirstOrDefault();

            if(firstDevice == null)
            {
                throw new Exception("Where is my bridge pro?");
            }

            IPAddress bridgeAddress = firstDevice.IPAddress;
            var cancelToken = new CancellationToken();

            using (var connector = new BridgeSslStreamConnector(bridgeAddress, cancelToken))
            {
                try
                {
                    bool isConnected = await connector.Connect();

                    await Task.Delay(2000);

                    for (var i = 0; i < 10; i++)
                    {
                        switch (i)
                        {
                            case 4:
                                connector.GetDevices();
                                break;
                            case 1:
                            default:
                                connector.Ping();
                                break;
                        }

                        await Task.Delay(2000);


                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }

            }

        }
        
    }

}


