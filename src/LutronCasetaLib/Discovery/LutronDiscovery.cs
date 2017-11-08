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
    public class LutronDiscovery
    {
        const string LUTRON_SERVICE_MDNS = "_lutron._tcp.local.";
        const string CODE_VERSION = "CODEVER";
        const string MAC_ADDRESS = "MACADDR";

        public static async Task<IEnumerable<LutronDeviceInfo>> DiscoverAllLutronDevices()
        {
            var responses = await ZeroconfResolver.ResolveAsync(LUTRON_SERVICE_MDNS);
            return IterateDiscoveryResponses(responses);
        }

        private static IEnumerable<LutronDeviceInfo> IterateDiscoveryResponses(IReadOnlyList<IZeroconfHost> responses)
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
