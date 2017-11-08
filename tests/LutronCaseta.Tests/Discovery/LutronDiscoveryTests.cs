using LutronCaseta.Discovery;
using LutronCaseta.Tests.Mocks.Discovery;
using System.Linq;
using Xunit;

namespace LutronCaseta.Tests
{
    public class LutronDiscoveryTests
    {
        [Fact]
        public async void given_call_to_discover_devices_should_call_resolve_using_lutron_dns_url()
        {
            // assemble
            var mockAggregate = new LutronDiscoveryMockAggregate()
                .ResolversResolveMethodGetsCalledReturningEmpty();

            // apply
            var discovery = mockAggregate.Create();
            var results = await discovery.DiscoverAllLutronDevices();

            // assert
            Assert.Equal(LutronDiscovery.LUTRON_SERVICE_MDNS, mockAggregate.TestState.ProtocolCalled);
        }

        [Fact]
        public async void given_call_to_discover_devices_should_return_same_number_of_devices()
        {
            // assemble
            var mockAggregate = new LutronDiscoveryMockAggregate()
                .ResolversResolveMethodGetsCalledReturningTwoEntries();

            // apply
            var discovery = mockAggregate.Create();
            var results = await discovery.DiscoverAllLutronDevices();

            // assert
            Assert.Equal(2, results.Count());
            Assert.Contains(results, a => a.IPAddress.ToString() == "192.168.99.123");
            Assert.Contains(results, a => a.IPAddress.ToString() == "192.168.99.124");
        }
    }
}
