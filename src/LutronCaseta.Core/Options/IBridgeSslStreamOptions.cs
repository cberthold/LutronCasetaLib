using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using LutronCaseta.Core.Logging;

namespace LutronCaseta.Core.Options
{
    public interface IBridgeSslStreamOptions
    {
        IPAddress BridgeAddress { get; set; }
        int BridgePort { get; set; }
        Func<string, X509CertificateCollection, X509Certificate, string[], X509Certificate> LocalCertificateSelectionPolicy { get; set; }
        ILogging Logging { get; set; }
        int ReadTimeout { get; set; }
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerValidationPolicy { get; set; }
        int WriteTimeout { get; set; }

        void Validate();
    }
}