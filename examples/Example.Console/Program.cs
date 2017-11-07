using LutronCaseta.Connectors;
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
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Task task = Task.Run(RunMe);
            Console.ReadKey();
        }




        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            var exception = e.Exception;
            ExceptionHandler.Handle(exception);
        }

        public static async Task EnumerateAllServicesFromAllHosts()
        {
            var responses = await ZeroconfResolver.ResolveAsync("_lutron._tcp.local.");
            foreach (var resp in responses)
                Console.WriteLine(resp);
        }

        public static async Task RunMe()
        {
            await EnumerateAllServicesFromAllHosts();


            IPAddress bridgeAddress = IPAddress.Parse("192.168.99.123");
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

    public class AsyncSynchronizationContext : SynchronizationContext
    {
        public override void Send(SendOrPostCallback callback, object state)
        {
            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }

    public static class ExceptionHandler
    {
        public static void Handle(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}


