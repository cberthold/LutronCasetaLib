using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LutronCaseta.Connectors.BouncyTls
{
    [Obsolete]
    public class BridgeTlsAuthentication : TlsAuthentication
    {
        readonly TlsClientContext context;
        
        public BridgeTlsAuthentication(TlsClientContext context)
        {
            this.context = context;
        }

        #region TlsAuthentication implementations

        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
        {
            var extraction = new CertificateExtraction();
            var certificate = extraction.FindCertificate(certificateRequest);
            var hash = new SignatureAndHashAlgorithm(HashAlgorithm.sha256, SignatureAlgorithm.rsa);
            var credentials = new DefaultTlsSignerCredentials(context, certificate, extraction.PrivateKey, hash);
            return credentials;
        }

        

        public void NotifyServerCertificate(Certificate serverCertificate)
        {

        }

        #endregion

        
    }
}
