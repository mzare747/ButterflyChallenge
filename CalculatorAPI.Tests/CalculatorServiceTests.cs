using CalculatorAPI.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace CalculatorAPI.Tests
{
    public class CalculatorServiceTests
    {
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<CalculatorService>> _mockLogger;
        private readonly CalculatorService _calculatorService;

        public CalculatorServiceTests()
        {
            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<CalculatorService>>();
            _calculatorService = new CalculatorService(_mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CalculateAsync_MustReturnCachedResult_WhenCacheHit()
        {
            // Arrange
            string cacheKey = "2_+_3";
            _mockCache.Setup(c => c.GetAsync(cacheKey, default))
                .ReturnsAsync(Encoding.UTF8.GetBytes("5"));

            // Act
            double result = await _calculatorService.CalculateAsync(2, 3, "+");

            // Assert
            Assert.Equal(5, result);
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cache hit for operation")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task CalculateAsync_MustCalculateAndCacheResult_WhenCacheMiss()
        {
            // Arrange
            string cacheKey = "2_+_3";
            _mockCache.Setup(c => c.GetAsync(cacheKey, default))
                .ReturnsAsync((byte[])null);

            // Act
            double result = await _calculatorService.CalculateAsync(2, 3, "+");

            // Assert
            Assert.Equal(5, result);
            _mockCache.Verify(c => c.SetAsync(
                cacheKey,
                It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "5"),
                It.IsAny<DistributedCacheEntryOptions>(),
                default),
                Times.Once);

            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Calculated result for 2 + 3")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task CalculateAsync_MustThrowDivideByZeroException_WhenDividingByZero()
        {
            // Arrange
            string cacheKey = "2_/_0";
            _mockCache.Setup(c => c.GetAsync(cacheKey, default))
                .ReturnsAsync((byte[])null);

            // Act & Assert
            await Assert.ThrowsAsync<DivideByZeroException>(() => _calculatorService.CalculateAsync(2, 0, "/"));
        }

        [Fact]
        public async Task CalculateAsync_MustThrowInvalidOperationException_WhenOperationIsInvalid()
        {
            // Arrange
            string cacheKey = "2_?_3";
            _mockCache.Setup(c => c.GetAsync(cacheKey, default))
                .ReturnsAsync((byte[])null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _calculatorService.CalculateAsync(2, 3, "?"));
        }
    }
}