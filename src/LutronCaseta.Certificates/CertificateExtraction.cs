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

namespace LutronCaseta
{
    public class CertificateExtraction
    {
        public IEnumerable<X509Certificate> Certificates { get; private set; }
        public AsymmetricKeyParameter PrivateKey { get; private set; }
        public Pkcs12Store Pkcs12Store { get; private set; }
        public System.Security.Cryptography.X509Certificates.X509Certificate2 KeyedCertificate { get; private set; }

        readonly X509CertificateParser certificatePemParser = new X509CertificateParser();
        readonly PrivateKeyParser privateKeyParser = new PrivateKeyParser();

        public string FilePath { get; private set; } = @"lutronbridge.pem";
        public CertificateExtraction(string filePath = null)
        {
            if (filePath != null)
            {
                FilePath = filePath;
            }
            ParseKeySets();
        }
        
        void ParseKeySets()
        {
            var fi = new FileInfo(FilePath);
            long contentLength = fi.Length;
            using (var ms = new MemoryStream((int)contentLength))
            {
                using (var fs = fi.OpenRead())
                {
                    fs.CopyTo(ms);
                }

                ms.Position = 0;
                Certificates = certificatePemParser.ReadCertificates(ms).Cast<X509Certificate>();
                ms.Position = 0;
                PrivateKey = privateKeyParser.ReadPrivateKey(ms);

            }

            var cert1 = Certificates.ElementAt(0);

            string alias = "bridgePro1";
            var pkcs12Store = Pkcs12Store = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(cert1);
            pkcs12Store.SetCertificateEntry(alias, certEntry);
            var certKey = PrivateKey;
            pkcs12Store.SetKeyEntry(alias, new AsymmetricKeyEntry(certKey), new[] { certEntry });

            using (MemoryStream pfxStream = new MemoryStream())
            {
                pkcs12Store.Save(pfxStream, null, new SecureRandom());
                pfxStream.Seek(0, SeekOrigin.Begin);
                KeyedCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxStream.ToArray());
            }
        }
    }
}
