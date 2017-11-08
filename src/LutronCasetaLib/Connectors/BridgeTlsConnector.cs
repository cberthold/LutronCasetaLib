using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using LutronCaseta.Connectors.BouncyTls;

namespace LutronCaseta.Connectors
{
    [Obsolete]
    public class BridgeTlsConnector : IDisposable
    {
        public const int DEFAULT_TLS_BRIDGE_PORT = 8081;
        #region host information

        public IPAddress BridgeAddress { get; private set; }
        public int BridgePort { get; private set; } = DEFAULT_TLS_BRIDGE_PORT;
        public string Username { get; private set; } = "lutron";
        public string Password { get; private set; } = "integration";
        public CancellationTokenSource TokeSource { get; private set; } = new CancellationTokenSource();

        TcpClient tcpClientSend;
        TlsClientProtocol protocolSend;
        TcpClient tcpClientReceive;
        TlsClientProtocol protocolReceive;
        IObservable<ArraySegment<byte>> readObservable;
        IObserver<ArraySegment<byte>> writeObservable;
        //StreamWriter writer;
        //StreamReader reader;

        public async Task<bool> Connect()
        {

            tcpClientReceive = new TcpClient();
            protocolReceive = await ConnectTcpClientToTls(tcpClientReceive);
            ListenForData(protocolReceive);

            tcpClientSend = new TcpClient();
            protocolSend = await ConnectTcpClientToTls(tcpClientSend);
            ListenForWrite(protocolSend);


            return true;
        }

        private async Task<TlsClientProtocol> ConnectTcpClientToTls(TcpClient client)
        {
            LingerOption lingerOption = new LingerOption(false, 0); // Make sure that on call to Close(), connection is closed immediately even if some data is pending.
            client.LingerState = lingerOption;

            await client.ConnectAsync(BridgeAddress.ToString(), BridgePort);

            var clientProtocol = new TlsClientProtocol(client.GetStream(), new SecureRandom());
            clientProtocol.Connect(new BridgeTlsClient());
            return clientProtocol;
        }

        private void ListenForWrite(TlsClientProtocol protocol)
        {
            writeObservable = protocol.Stream
                .ToStreamObserver(TokeSource.Token);

        }

        public void ListenForData(TlsClientProtocol protocol)
        {

            readObservable = protocol.Stream
                .ToStreamObservable(1)
                .SubscribeOn(NewThreadScheduler.Default);

            readObservable
                .Subscribe((segment) =>
                {
                    var str = Encoding.UTF8.GetString(segment.Array);
                    Console.Write(str);
                },
                //(e) => ExceptionHandler.Handle(e),
                () => Console.WriteLine("Done"), TokeSource.Token);
        }

        private void WriteString(string stringToWrite)
        {
            var writeBuffer = Encoding.UTF8.GetBytes(stringToWrite);
            var value = DisposableValue.Create(new ArraySegment<byte>(writeBuffer, 0, writeBuffer.Length), DisposableValue<ArraySegment<byte>>.Empty);
            writeObservable.OnNext(value.Value);
            Console.WriteLine("Sent");
        }

        public void Ping()
        {
            string ping = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/server/1/status/ping\"}}";
            WriteString(ping);

        }

        public void GetDevices()
        {
            string deviceCommand = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/device\"}}";

            WriteString(deviceCommand);
        }


        #endregion



        public BridgeTlsConnector(IPAddress bridgeAddress, CancellationToken token = default(CancellationToken))
        {
            BridgeAddress = bridgeAddress;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    //Client.Dispose();
                    //Client = null;
                }

                // free unmanaged resources.
                // set large fields to null.

                disposedValue = true;
            }
        }

        ~BridgeTlsConnector()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        
    }

    
}
