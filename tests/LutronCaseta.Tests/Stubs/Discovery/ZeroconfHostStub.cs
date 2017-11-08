using System;
using System.Collections.Generic;
using System.Text;
using Zeroconf;

namespace LutronCaseta.Tests.Stubs.Discovery
{
    public class ZeroconfHostStub : IZeroconfHost
    {
        public string DisplayName { get; set; }

        public string Id { get; set; }

        public string IPAddress { get; set; }

        public IReadOnlyList<string> IPAddresses { get; set; }

        public IReadOnlyDictionary<string, IService> Services { get; set; }
    }
}
