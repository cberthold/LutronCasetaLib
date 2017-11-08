using LutronCaseta.Discovery;
using LutronCaseta.Tests.Mocks.Discovery;
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
    }
}
