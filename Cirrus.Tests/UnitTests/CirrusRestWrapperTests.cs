using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Infrastructure;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Cirrus.Tests.UnitTests
{
    public class CirrusRestWrapperTests
    {
        private const string SampleApplicationKey = "application-key";
        private const string SampleMacAddress = "2F:29:A3:78:A1:B5";
        private const string SampleApiKey = "super-secret-api-key-1";

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task FetchDeviceDataAsync_Success()
        {
            var mockCirrusService = new Mock<API.ICirrusService>();

            IEnumerable<Device> expectedDeviceList = new List<Device>
                { new Device { MacAddress = SampleMacAddress } };

            mockCirrusService.Setup(
                x => x.Fetch<Device>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(
                ServiceResponse.Ok<IEnumerable<Device>>(expectedDeviceList)
            ).Verifiable(); 

            using var cirrusRestWrapper = new CirrusRestWrapper(
                options: GetCirrusConfig(SampleApplicationKey, SampleMacAddress, SampleApiKey),
                service: mockCirrusService.Object
            );

            var results = await cirrusRestWrapper.FetchDeviceDataAsync(
                System.DateTimeOffset.UtcNow,
                limit: 100,
                cancellationToken: CancellationToken.None
            );

            Assert.NotNull(results);
            Assert.IsNotEmpty(results);
            
            mockCirrusService.Verify();

            Assert.AreEqual(expectedDeviceList, results);
        }

        [Test]
        public async Task FetchDeviceDataAsync_Success_ZeroLimit()
        {
            var mockCirrusService = new Mock<API.ICirrusService>();

            using var cirrusRestWrapper = new CirrusRestWrapper(
                options: GetCirrusConfig(SampleApplicationKey, SampleMacAddress, SampleApiKey),
                service: mockCirrusService.Object
            );

            var results = await cirrusRestWrapper.FetchDeviceDataAsync(DateTimeOffset.UtcNow, 0, CancellationToken.None);

            Assert.NotNull(results);
            Assert.IsEmpty(results);

            mockCirrusService.Verify();
        }

        [Test]
        public async Task FetchDeviceDataAsync_ServiceResponse_Failure()
        {
            var mockCirrusService = new Mock<API.ICirrusService>();

            mockCirrusService.Setup(
                x => x.Fetch<Device>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(
                ServiceResponse.Fail<IEnumerable<Device>>("Error occurred!")
            ).Verifiable(); 

            using var cirrusRestWrapper = new CirrusRestWrapper(
                options: GetCirrusConfig(SampleApplicationKey, SampleMacAddress, SampleApiKey),
                service: mockCirrusService.Object
            );

            var results = await cirrusRestWrapper.FetchDeviceDataAsync(DateTimeOffset.UtcNow, 100, CancellationToken.None);

            Assert.NotNull(results);
            Assert.IsEmpty(results);

            mockCirrusService.Verify();
        }

        [TestCase("", SampleMacAddress, SampleApiKey)]
        [TestCase(SampleApplicationKey, "", SampleApiKey)]
        [TestCase(SampleApplicationKey, SampleMacAddress, "")]
        public void FetchDeviceDataAsync_ArgumentValidation(string applicationKey, string macAddress, string apiKey)
        {
            var mockCirrusService = new Mock<API.ICirrusService>();

            using var cirrusRestWrapper = new CirrusRestWrapper(
                options: GetCirrusConfig(applicationKey, macAddress, apiKey),
                service: mockCirrusService.Object
            );

            var exception = Assert.ThrowsAsync<ArgumentException>(
                async () => await cirrusRestWrapper.FetchDeviceDataAsync(DateTimeOffset.UtcNow, 100, CancellationToken.None)
            );

            Assert.Contains(
                exception.ParamName,
                new List<string>
                {
                    nameof(cirrusRestWrapper.ApplicationKey),
                    nameof(cirrusRestWrapper.ApiKey),
                    nameof(cirrusRestWrapper.MacAddress)
                }
            );
        }

        private static IOptions<CirrusConfig> GetCirrusConfig(string applicationKey, string macAddress, params string[] apiKeys)
        {
            return Options.Create<CirrusConfig>(
                new CirrusConfig
                {
                    ApplicationKey = applicationKey,
                    MacAddress = macAddress,
                    ApiKeys = new(apiKeys),
                }
            );
        }
    }
}