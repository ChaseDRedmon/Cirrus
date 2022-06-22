using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.API;
using Cirrus.Extensions;
using Cirrus.Models;
using Cirrus.Tests.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Cirrus.Tests.IntegrationTests
{
    [TestFixture]
    public class RestIntegrationTests
    {
        private string Device { get; set; }
        private string ApiKey { get; set; }
        private string ApplicationKey { get; set; }

        private ICirrusRestWrapper sut;
        
        private DateTimeOffset invalidDateTime = new(2019, 1, 1, 0, 0, 0, TimeSpan.Zero); // This is a known date where there is no information available
        
        [OneTimeSetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("connections.json", optional: false, reloadOnChange: false)
                .Build();

            Device = config.GetValue<string>(nameof(CirrusConfig.MacAddress));
            ApiKey = config.GetValue<string>(nameof(CirrusConfig.ApiKeys));
            ApplicationKey = config.GetValue<string>(nameof(CirrusConfig.ApplicationKey));

            var serviceCollection = new ServiceCollection()
                .AddCirrusServices(cirrusConfig =>
                {
                    cirrusConfig.ApiKeys = new List<string> { ApiKey };
                    cirrusConfig.ApplicationKey = ApplicationKey;
                    cirrusConfig.MacAddress = Device;
                });

            var provider = serviceCollection.BuildServiceProvider();
            sut = provider.GetRequiredService<ICirrusRestWrapper>();
        }

        [Test, NonParallelizable]
        public async Task Invalid_Ambient_Weather_Date_Query_Should_Return_False()
        {
            // Arrange
            var invalidDateTime = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero); // This is a known date where there is no information available
            
            // Act
            var result = await sut.DoesDeviceDataExist(invalidDateTime, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
        
        [Test, NonParallelizable]
        public async Task Valid_Ambient_Weather_Date_Query_Should_Return_True()
        {
            // Arrange
            var validDateTime = new DateTimeOffset(DateTime.Today); // The Weather Station Is Online and will be reporting information
            
            // Act
            var result = await sut.DoesDeviceDataExist(validDateTime, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Test, NonParallelizable]
        public async Task Does_Ambient_Weather_Return_All_Information_Requested()
        {
            // Arrange
            var validDateTime = new DateTimeOffset(DateTime.Today.AddDays(-2)); // The Weather Station Is Online and will be reporting information
            
            // Act
            var result = await sut.DoesDeviceDataExist(validDateTime, CancellationToken.None);

            // Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task Does_Ambient_Weather_Return_Information_Requested_With_Cancellation()
        {
            // Arrange
            var validDateTime = new DateTimeOffset(DateTime.Today.AddDays(-2)); // The Weather Station Is Online and will be reporting information
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            
            // Act
            var result = await sut.DoesDeviceDataExist(validDateTime, cts.Token);

            // Assert
            Assert.True(result);
        }
        
        [Test]
        public async Task Does_Ambient_Weather_Return_Information_Requested_With_Cancellation_And_Timeout()
        {
            // Arrange
            var validDateTime = new DateTimeOffset(DateTime.Today.AddDays(-2)); // The Weather Station Is Online and will be reporting information
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(50)); // This timeout is too short to allow the request to complete
            
            // Act
            var result = await sut.DoesDeviceDataExist(validDateTime, cts.Token);

            // Assert
            Assert.True(result);
        }

        private static IOptions<CirrusConfig> GetCirrusConfig(string applicationKey, string macAddress, params string[] apiKeys)
        {
            return Options.Create<CirrusConfig>(
                new CirrusConfig
                {
                    ApplicationKey = applicationKey,
                    MacAddress = macAddress,
                    ApiKeys = new List<string>(apiKeys),
                }
            );
        }
        
    }
}