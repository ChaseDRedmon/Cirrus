using System;
using System.Threading;
using Cirrus.Wrappers;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Cirrus.Tests.UnitTests
{
    public class CirrusRestTests
    {
        private string Device { get; set; }
        private string ApiKey { get; set; }
        private string ApplicationKey { get; set; }
        
        private Mock<ICirrusRestWrapper> _cirrusRestWrapperMock { get; set; }
        private Mock<ILogger> _loggerMock { get; set; } 
        
        private DateTimeOffset invalidDateTime = new(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        [SetUp]
        public void Setup()
        {
            _cirrusRestWrapperMock = new Mock<ICirrusRestWrapper>();
            _loggerMock = new Mock<ILogger>();

            var mockCirrusWrapper = new Mock<CirrusWrapper>();
            mockCirrusWrapper.Setup(mock => mock.FetchDeviceHistory(1, CancellationToken.None, false, true, 288));

            


        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}