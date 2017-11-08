using LutronCaseta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace LutronCaseta.Discovery
{
    /// <summary>
    /// Discovers Lutron devices via MDNS Bonjour multicast calls
    /// </summary>
    public sealed class LutronDiscovery
    {
        public const string LUTRON_SERVICE_MDNS = "_lutron._tcp.local.";
        public const string CODE_VERSION = "CODEVER";
        public const string MAC_ADDRESS = "MACADDR";

        public static IDiscoveryResolver Resolver { get; set; } = new ZeroconfDiscoveryResolver();

        public async Task<IEnumerable<ILutronDeviceInfo>> DiscoverAllLutronDevices()
        {
            // resolve out classes using zeroconf
            var responses = await Resolver.ResolveAsync(LUTRON_SERVICE_MDNS);
            // iterate the responses and return them as ILutronDeviceInfo
            return IterateDiscoveryResponses(responses);
        }

        private IEnumerable<ILutronDeviceInfo> IterateDiscoveryResponses(IReadOnlyList<IZeroconfHost> responses)
        {
            foreach (var resp in responses)
            {
                IPAddress ipAddress = IPAddress.Parse(resp.IPAddress);
                string codeVersion = string.Empty;
                string devClass = string.Empty;
                string macAddress = string.Empty;
                IService lutronService = null;

                if (resp.Services.ContainsKey(LUTRON_SERVICE_MDNS))
                {
                    lutronService = resp.Services[LUTRON_SERVICE_MDNS];
                }
                else
                {
                    lutronService = resp.Services.Values.FirstOrDefault();
                }

                // don't know if properties could ever end up null
                // yields are easier to deal with this way
                var properties = lutronService?.Properties ?? new List<IReadOnlyDictionary<string, string>>();

                foreach (var propertyDictionary in properties)
                {
                    // get the code version if one of the properties contains it
                    if (propertyDictionary.ContainsKey(CODE_VERSION))
                    {
                        codeVersion = propertyDictionary[CODE_VERSION];
                    }
                    // get the MAC address if one of the properties contains it
                    if (propertyDictionary.ContainsKey(MAC_ADDRESS))
                    {
                        macAddress = propertyDictionary[MAC_ADDRESS];
                    }
                }

                // set our results into our device info and yield them to the iterator
                yield return new LutronDeviceInfo(ipAddress, codeVersion, macAddress);
            }
        }
    }
}
