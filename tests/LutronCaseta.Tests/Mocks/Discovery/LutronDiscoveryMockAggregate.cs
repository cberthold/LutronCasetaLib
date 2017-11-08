using LutronCaseta.Discovery;
using LutronCaseta.Tests.Stubs.Discovery;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Zeroconf;

namespace LutronCaseta.Tests.Mocks.Discovery
{
    /// <summary>
    /// Aggregates mocks for LutronDiscovery and factory creates it for tests
    /// </summary>
    public class LutronDiscoveryMockAggregate
    {
        #region State

        public class State
        {
            public string ProtocolCalled { get; set; }
        }

        public State TestState { get; private set; } = new State();

        #endregion

        #region Mocks

        public Mock<IDiscoveryResolver> ResolverMock { get; } = new Mock<IDiscoveryResolver>();

        #endregion

        #region Mock Implementations

        public IDiscoveryResolver Resolver => ResolverMock.Object;

        #endregion

        #region Factory

        public LutronDiscovery Create()
        {
            LutronDiscovery.Resolver = Resolver;
            var discovery = new LutronDiscovery();
            return discovery;
        }

        #endregion

        #region Builder methods

        internal LutronDiscoveryMockAggregate ResolversResolveMethodGetsCalledReturningEmpty()
        {
            ResolverMock.Setup(a => a.ResolveAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Action<IZeroconfHost>>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, TimeSpan, int, int, Action<IZeroconfHost>, CancellationToken>(
                (protocol, scanTime, retries, retryDelay, callback, token) =>
                {
                    TestState.ProtocolCalled = protocol;
                })
                .ReturnsAsync(() => new List<IZeroconfHost>());

            return this;
        }

        internal LutronDiscoveryMockAggregate ResolversResolveMethodGetsCalledReturningTwoEntries()
        {
            ResolverMock.Setup(a => a.ResolveAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Action<IZeroconfHost>>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, TimeSpan, int, int, Action<IZeroconfHost>, CancellationToken>(
                (protocol, scanTime, retries, retryDelay, callback, token) =>
                {
                    TestState.ProtocolCalled = protocol;
                })
                .ReturnsAsync(() => new List<IZeroconfHost>()
                {
                    BuildHost("192.168.99.123", "00:00:00:00:01", "00.00.00"),
                    BuildHost("192.168.99.124", "00:00:00:00:02", "00.00.00"),
                });

            return this;
        }

        #endregion


        #region private methods

        private IZeroconfHost BuildHost(string ipAddress, string macaddress, string version)
        {
            return new ZeroconfHostStub
            {
                DisplayName = "Lutron Status",
                Id = ipAddress,
                IPAddress = ipAddress,
                IPAddresses = new List<string>() { ipAddress },
                Services = new Dictionary<string, IService>(),
        };
    }

    #endregion
}
}
