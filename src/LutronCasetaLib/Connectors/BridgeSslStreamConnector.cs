using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LutronCaseta.Connectors
{
    public class BridgeSslStreamConnector : IDisposable
    {

        #region host information and public props

        public const int DEFAULT_TLS_BRIDGE_PORT = 8081;

        public IPAddress BridgeAddress { get; private set; }
        public int BridgePort { get; private set; } = DEFAULT_TLS_BRIDGE_PORT;
        public CancellationToken CancelToken { get; private set; }

        #endregion

        #region private properties

        SslStream sslStream;
        IObservable<ArraySegment<byte>> readObservable;
        IObserver<ArraySegment<byte>> writeObservable;

        #endregion

        #region Constructor

        public BridgeSslStreamConnector(IPAddress bridgeAddress, CancellationToken token = default(CancellationToken))
        {
            BridgeAddress = bridgeAddress;
            CancelToken = token;
        }

        #endregion
        
        #region Connect to stream

        public async Task<bool> Connect()
        {
            sslStream = await ConnectToSslStream();
            ListenForData(sslStream);
            ListenForWrite(sslStream);
            return true;
        }

        private async Task<SslStream> ConnectToSslStream()
        {
            var tcpClient = new TcpClient(BridgeAddress.ToString(), BridgePort);
            var extraction = new CertificateExtraction();
            var serverValidation = new RemoteCertificateValidationCallback((o, cert, chain, policy) =>
            {
                return true;
            });
            var localSelection = new LocalCertificateSelectionCallback((o, host, localCerts, remoteCerts, acceptableUsers) =>
            {
                return extraction.KeyedCertificate;
            });
            var stream = new SslStream(tcpClient.GetStream(), false, serverValidation, localSelection, EncryptionPolicy.RequireEncryption);
            await stream.AuthenticateAsClientAsync(BridgeAddress.ToString(), null, SslProtocols.Tls12, true);
            stream.ReadTimeout = 5000;
            stream.WriteTimeout = 5000;
            return stream;
        }

        #endregion

        #region Observable listeners

        private void ListenForWrite(SslStream sslStream)
        {
            writeObservable = sslStream
                .ToStreamObserver(CancelToken);
        }

        public void ListenForData(SslStream sslStream)
        {

            readObservable = sslStream
                .ToStreamObservable(9999)
                .SubscribeOn(NewThreadScheduler.Default);

            readObservable
                .Subscribe((segment) =>
                {
                    var offset = segment.Offset;
                    var count = segment.Count;
                    var str = Encoding.UTF8.GetString(segment.Array, offset, count);
                    Console.Write(str);
                },
                //(e) => ExceptionHandler.Handle(e),
                () => Console.WriteLine("Done"), CancelToken);
        }

        #endregion

        #region Write commands

        private void WriteString(string stringToWrite)
        {
            Console.WriteLine($"Writing: {stringToWrite}");
            var writeBuffer = Encoding.UTF8.GetBytes(stringToWrite);
            //var value = DisposableValue.Create(new ArraySegment<byte>(writeBuffer, 0, writeBuffer.Length), DisposableValue<ArraySegment<byte>>.Empty);
            writeObservable.OnNext(new ArraySegment<byte>(writeBuffer, 0, writeBuffer.Length));
            Console.WriteLine("Sent");
        }

        public void Ping()
        {
            string ping = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/server/1/status/ping\"}}\n";
            WriteString(ping);

        }

        public void GetDevices()
        {
            string deviceCommand = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/device\"}}\n";

            WriteString(deviceCommand);
        }

        #endregion

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

        ~BridgeSslStreamConnector()
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
