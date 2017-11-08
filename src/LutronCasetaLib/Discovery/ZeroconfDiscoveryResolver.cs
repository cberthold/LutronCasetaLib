using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace LutronCaseta.Discovery
{
    /// <summary>
    /// Discovery resolver for Zeroconf
    /// </summary>
    public class ZeroconfDiscoveryResolver : IDiscoveryResolver
    {
        public Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(string protocol, TimeSpan scanTime = default(TimeSpan), int retries = 2, int retryDelayMilliseconds = 2000, Action<IZeroconfHost> callback = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ZeroconfResolver.ResolveAsync(protocol, scanTime, retries, retryDelayMilliseconds, callback, cancellationToken);
        }
    }
}
