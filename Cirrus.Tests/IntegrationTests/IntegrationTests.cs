using System;
using Cirrus.Tests.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Cirrus.Tests.IntegrationTests
{
    [TestFixture]
    public class RestIntegrationTests
    {
        private string Device { get; set; }
        private string ApiKey { get; set; }
        private string ApplicationKey { get; set; }

        private CirrusRestWrapper sut;
        private CirrusRestWrapper sut1;
        private CirrusRestWrapper sut2;
        
        private DateTimeOffset invalidDateTime = new(2019, 1, 1, 0, 0, 0, TimeSpan.Zero); // This is a known date where there is no information available
        
        [OneTimeSetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("connections.json", optional: false, reloadOnChange: true)
                .Build();

            Device = config.GetValue<string>(nameof(Credentials.MacAddress));
            ApiKey = config.GetValue<string>(nameof(Credentials.ApiKey));
            ApplicationKey = config.GetValue<string>(nameof(Credentials.ApplicationKey));
        }

        /*[Test]
        public async Task Invalid_Ambient_Weather_Date_Query_Should_Return_False()
        {
            // Arrange
            var invalidDateTime = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero); // This is a known date where there is no information available

            
            
            // Act
            var result = await sut.DoesDeviceDataExist(invalidDateTime, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
        
        [Test]
        public async Task Null_Arguments_Throw_Argument_Exception()
        {
            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => 
                await sut.DoesDeviceDataExist(invalidDateTime, CancellationToken.None));
            
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut1.DoesDeviceDataExist(invalidDateTime, CancellationToken.None));

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut2.DoesDeviceDataExist(invalidDateTime, CancellationToken.None));
        }*/
    }
}