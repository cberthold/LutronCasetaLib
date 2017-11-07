using LutronCaseta;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Connectors.BouncyTls
{
    public class BridgeTlsClient : DefaultTlsClient
    {
        public override TlsAuthentication GetAuthentication()
        {
            return new BridgeTlsAuthentication(mContext);
        }
    }
}
