using LutronCaseta.Core.Exceptions;
using LutronCaseta.Core.Logging;
using LutronCaseta.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LutronCaseta.Core.Options
{
    public class BridgeSslStreamOptions : IBridgeSslStreamOptions
    {

        public const int DEFAULT_TLS_BRIDGE_PORT = 8081;

        #region Option Properties

        /// <summary>
        /// IP Address for Lutron Bridge
        /// </summary>
        public IPAddress BridgeAddress { get; set; }

        /// <summary>
        /// TCP port for the TLS connections on Lutron Bridge
        /// </summary>
        public int BridgePort { get; set; } = DEFAULT_TLS_BRIDGE_PORT;

        /// <summary>
        /// Read Timeout for TLS stream
        /// </summary>
        public int ReadTimeout { get; set; } = 5000;

        /// <summary>
        /// Write Timeout for TLS stream
        /// </summary>
        public int WriteTimeout { get; set; } = 5000;

        /// <summary>
        /// Logging interface
        /// </summary>
        public ILogging Logging { get; set; } = NullLogging.Instance;

        /// <summary>
        /// Function that validates the server certificate - default is always TRUE
        /// </summary>
        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerValidationPolicy { get; set; } = DefaultServerValidationPolicy();

        /// <summary>
        /// Function that selects the private key certificate to authenticate with the Lutron Bridge
        /// </summary>
        public Func<string, X509CertificateCollection, X509Certificate, string[], X509Certificate> LocalCertificateSelectionPolicy { get; set; } = DefaultLocalCertificateSelectionPolicy();

        #endregion

        #region Constructor

        public BridgeSslStreamOptions(IPAddress bridgeAddress)
        {
            BridgeAddress = bridgeAddress;
        }

        #endregion

        #region Default Policies

        public static Func<X509Certificate, X509Chain, SslPolicyErrors, bool> DefaultServerValidationPolicy()
        {
            return (cert, chain, policyErrors) =>
            {
                return true;
            };
        }

        public static Func<string, X509CertificateCollection, X509Certificate, string[], X509Certificate> DefaultLocalCertificateSelectionPolicy()
        {
            return (host, localCerts, remoteCerts, acceptableUsers) =>
            {
                throw new NotImplementedException($"{nameof(DefaultLocalCertificateSelectionPolicy)} must be implemented to select a certificate");
            };
        }

        #endregion

        #region Option Validation

        public void Validate()
        {
            if (BridgeAddress == null)
            {
                throw new BridgeOptionValidationException($"{nameof(BridgeAddress)} cannot be null");
            }

            if (!(BridgePort > 0 && BridgePort < 65535))
            {
                throw new BridgeOptionValidationException($"{nameof(BridgePort)} must be between 0 and 65535");
            }

            if (ServerValidationPolicy == null)
            {
                throw new BridgeOptionValidationException($"{nameof(ServerValidationPolicy)} cannot be null");
            }

            if (LocalCertificateSelectionPolicy == null)
            {
                throw new BridgeOptionValidationException($"{nameof(LocalCertificateSelectionPolicy)} cannot be null");
            }
        }

        #endregion

    }
}
