using PrimS.Telnet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LutronCaseta.Connectors
{
    public class BridgeTelnetConnector : IDisposable
    {
        public const int DEFAULT_TELNET_PORT = 23;
        #region host information

        public IPAddress BridgeAddress { get; private set; }
        public int BridgePort { get; private set; } = DEFAULT_TELNET_PORT;
        public string Username { get; private set; } = "lutron";

        public Task<bool> Connect()
        {
            return Client.TryLoginAsync(Username, Password, 9000);
        }

        public string Password { get; private set; } = "integration";

        #endregion

        public Client Client { get; private set; }


        public BridgeTelnetConnector(IPAddress bridgeAddress, CancellationToken token = default(CancellationToken))
        {
            BridgeAddress = bridgeAddress;
            Client = CreateClient(token);
        }

        private Client CreateClient(CancellationToken token)
        {
            return new Client(BridgeAddress.ToString(), BridgePort, token);
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
                    Client.Dispose();
                    Client = null;
                }

                // free unmanaged resources.
                // set large fields to null.

                disposedValue = true;
            }
        }

        ~BridgeTelnetConnector()
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
