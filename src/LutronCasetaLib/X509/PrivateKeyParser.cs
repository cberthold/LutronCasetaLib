using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LutronCaseta
{
    public class PrivateKeyParser : Parser
    {
        public PrivateKeyParser() : base("RSA PRIVATE KEY") { }

        public AsymmetricKeyParameter ReadPrivateKey(
            byte[] input)
        {
            using (var stream = new MemoryStream(input, false))
            {
                return ReadPrivateKey(stream);
            }
        }
        public AsymmetricKeyParameter ReadPrivateKey(
                    Stream inStream)
        {
            if (inStream == null)
                throw new ArgumentNullException("inStream");
            if (!inStream.CanRead)
                throw new ArgumentException("inStream must be read-able", "inStream");

            var encodedKey = ReadEncodedObject(inStream);
            var obj = new PemReader(new StringReader(encodedKey)).ReadPemObject();
            var asn1 = Asn1Object.FromByteArray(obj.Content);
            var seq = (Asn1Sequence)asn1;
            RsaPrivateKeyStructure rsa = new RsaPrivateKeyStructure(seq);

            var pubSpec = new RsaKeyParameters(false, rsa.Modulus, rsa.PublicExponent);
            var privSpec = new RsaPrivateCrtKeyParameters(
                rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent,
                rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2,
                rsa.Coefficient);

            var keyPair = new AsymmetricCipherKeyPair(pubSpec, privSpec);
            return keyPair.Private;
        }
    }
    public class Parser
    {
        private readonly string _header1;
        private readonly string _header2;
        private readonly string _footer1;
        private readonly string _footer2;

        internal Parser(string type)
        {
            _header1 = "-----BEGIN " + type + "-----";
            _header2 = "-----BEGIN X509 " + type + "-----";
            _footer1 = "-----END " + type + "-----";
            _footer2 = "-----END X509 " + type + "-----";
        }

        private string ReadLine(
            Stream inStream)
        {
            int c;
            StringBuilder l = new StringBuilder();

            do
            {
                while (((c = inStream.ReadByte()) != '\r') && c != '\n' && (c >= 0))
                {
                    if (c == '\r')
                    {
                        continue;
                    }

                    l.Append((char)c);
                }
            }
            while (c >= 0 && l.Length == 0);

            if (c < 0)
            {
                return null;
            }

            return l.ToString();
        }

        public string ReadEncodedObject(
            Stream inStream)
        {
            string line;
            StringBuilder pemBuf = new StringBuilder();

            while ((line = ReadLine(inStream)) != null)
            {
                if (line.StartsWith(_header1) || line.StartsWith(_header2))
                {
                    pemBuf.AppendLine(line);
                    break;
                }
            }

            while ((line = ReadLine(inStream)) != null)
            {
                if (line.StartsWith(_footer1) || line.StartsWith(_footer2))
                {
                    pemBuf.AppendLine(line);
                    break;
                }

                pemBuf.AppendLine(line);

            }

            if (pemBuf.Length != 0)
            {
                return pemBuf.ToString();
            }

            return null;
        }
    }


}
