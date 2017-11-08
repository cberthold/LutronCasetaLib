using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace LutronCaseta.Discovery
{
    /// <summary>
    /// Represents the used contracts of ZeroconfResolver to support unit testing of LutronDiscovery class
    /// </summary>
    public interface IDiscoveryResolver
    {
        Task<IReadOnlyList<IZeroconfHost>> ResolveAsync(string protocol, TimeSpan scanTime = default(TimeSpan), int retries = 2, int retryDelayMilliseconds = 2000, Action<IZeroconfHost> callback = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
