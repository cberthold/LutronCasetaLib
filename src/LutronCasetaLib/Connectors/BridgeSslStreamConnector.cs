using LutronCaseta.Core.Commands.Write;
using LutronCaseta.Core.Connectors;
using LutronCaseta.Core.Exceptions;
using LutronCaseta.Core.Options;
using LutronCaseta.Core.Responses;
using LutronCaseta.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LutronCaseta.Connectors
{

    public class BridgeSslStreamConnector : IDisposable, IWriteProcessor, IBridgeConnector
    {

        #region host information and public props

        public BridgeSslStreamOptions Options { get; private set; }

        public CancellationToken CancelToken { get; private set; }

        public TcpClient TcpClient { get; private set; }
        public SslStream SslStream { get; private set; }
        public Subject<ICommuniqueType> Responses { get; } = new Subject<ICommuniqueType>();

        #endregion

        #region private properties

        IObservable<ArraySegment<byte>> readObservable;
        IObserver<ArraySegment<byte>> writeObservable;
        readonly ResponseMapper responseMapper = new ResponseMapper();

        #endregion

        #region Constructor

        public BridgeSslStreamConnector(IPAddress bridgeAddress, X509Certificate privateKeyCertificate, CancellationToken token = default(CancellationToken))
        {
            var ipAddress = bridgeAddress ?? throw new ArgumentNullException(nameof(bridgeAddress));
            Options = new BridgeSslStreamOptions(ipAddress)
            {
                LocalCertificateSelectionPolicy = (a1, a2, a3, a4) => privateKeyCertificate,
            };

            CancelToken = token;
        }

        public BridgeSslStreamConnector(BridgeSslStreamOptions options, CancellationToken token = default(CancellationToken))
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            CancelToken = token;
        }

        #endregion

        #region Connect to stream

        public async Task<bool> Connect()
        {
            var authenticated = await ConnectToSslStream();

            if (authenticated)
            {
                ListenForData(SslStream);
                ListenForWrite(SslStream);
            }
            return authenticated;
        }

        private async Task<bool> ConnectToSslStream()
        {
            var options = Options;

            // validate options
            options.Validate();

            // create the server validation policy to be used by the SSL stream
            var serverValidationPolicy = options.ServerValidationPolicy;
            var serverValidationCallback = new RemoteCertificateValidationCallback((o, cert, chain, policy) =>
            {
                return serverValidationPolicy(cert, chain, policy);
            });

            // create the local client certificate selection policy to be used by the SSL Stream
            var localCertificateSelectionPolicy = options.LocalCertificateSelectionPolicy;
            var localSelectionCallback = new LocalCertificateSelectionCallback((o, host, localCerts, remoteCerts, acceptableUsers) =>
            {
                return localCertificateSelectionPolicy(host, localCerts, remoteCerts, acceptableUsers);
            });

            // connect to Lutron Bridge TCP port
            string bridgeAddress = options.BridgeAddress.ToString();

            try
            {

                TcpClient = new TcpClient(AddressFamily.InterNetwork);
                await TcpClient.ConnectAsync(options.BridgeAddress, options.BridgePort);
            }
            catch (Exception ex)
            {
                CloseStreams();

                throw ex;
            }


            // open SslStream using TCP Client Stream to ride on
            var closeInnerStreamOnOuterStreamClose = false;
            var stream = SslStream = new SslStream(
                TcpClient.GetStream(),
                closeInnerStreamOnOuterStreamClose,
                serverValidationCallback,
                localSelectionCallback,
                EncryptionPolicy.RequireEncryption
                );

            // set the read and write timeouts on the ssl stream
            stream.ReadTimeout = options.ReadTimeout;
            stream.WriteTimeout = options.WriteTimeout;

            var isAuthenticated = false;

            try
            {
                // authenticate the client to the Lutron Bridge
                await stream.AuthenticateAsClientAsync(bridgeAddress, null, SslProtocols.Tls12, false);
                isAuthenticated = stream.IsAuthenticated;

            }
            catch (Exception ex)
            {
                // ensure not authenticated
                isAuthenticated = false;

                // rethrow
                throw ex;
            }
            finally
            {
                // make sure if we aren't authenticated for any reason
                // that we close out the streams
                if (!isAuthenticated)
                {
                    CloseStreams();
                }
            }

            return isAuthenticated;
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
                .ToStreamObservable(1)
                .SubscribeOn(NewThreadScheduler.Default);

            var encoding = Encoding.UTF8;
            readObservable
                .Select(a=> encoding.GetString(a.Array, a.Offset, a.Count))
                .Scan(String.Empty, (a, b) => (a.EndsWith("\n") ? "" : a) +  b)
                .Where(a => a.EndsWith("\n"))
                .Subscribe((str) =>
                {
                    Options.Logging.Debug(str);
                    var mappedObject = responseMapper.MapJsonResponse(str);
                    Responses.OnNext(mappedObject);
                },
                (e) => Responses.OnError(e),
                () =>
                {
                    Options.Logging.Debug("Read Subscription Done");
                    Responses.OnCompleted();
                }, CancelToken);
        }

        #endregion

        #region Mediate write commands

        void IWriteProcessor.ExecuteCommand(IWriteCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            // send the command
            command.SendCommand(this);
        }

        /// <summary>
        /// Write string method that the command uses to mediate to the writeObservable
        /// </summary>
        /// <param name="writeString"></param>
        void IWriteProcessor.WriteString(string writeString)
        {
            this.WriteStringInternal(writeString);
        }

        #endregion

        #region Write commands

        private void WriteStringInternal(string stringToWrite)
        {
            var logging = Options.Logging;
            logging.Debug($"Writing: {stringToWrite}");
            var writeBuffer = Encoding.UTF8.GetBytes(stringToWrite);
            //var value = DisposableValue.Create(new ArraySegment<byte>(writeBuffer, 0, writeBuffer.Length), DisposableValue<ArraySegment<byte>>.Empty);
            writeObservable.OnNext(new ArraySegment<byte>(writeBuffer, 0, writeBuffer.Length));
            logging.Debug("Sent");
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed state (managed objects).
                CloseStreams();

            }

            // free unmanaged resources.
            // set large fields to null.
            Options = null;
            disposedValue = true;

        }

        private void CloseStreams()
        {
            if (SslStream != null)
            {
                SslStream.Dispose();
                SslStream = null;
            }

            if (TcpClient != null)
            {
                TcpClient.Dispose();
                TcpClient = null;
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
