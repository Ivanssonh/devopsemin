using NSubstitute;
using SampleProject.Domain.ForeignExchange;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Domain.ForeignExchanges;
using System.Collections.Generic;
using Xunit;

namespace SampleProject.UnitTests.ForeignExchanges
{
    public class ForeignExhangeTests
    {
        [Fact]

        public void GetConversionRates_WhenCacheNotAvailable_ShouldReturnTwoValues()
        {
            //Arange
            var mockCacheStore = Substitute.For<ICacheStore>();
            var foreignExchange = new ForeignExchange(mockCacheStore);

            //Act
            var result = foreignExchange.GetConversionRates();

            //Assert
            Assert.Equal(2, result.Count);

        }

        [Fact]
        public void GetConversionRates_WhenCacheAvailable_ShouldReturnOneValue()
        {
            // Arrange
            var mockCacheStore = Substitute.For<ICacheStore>();
            var cachedRate = new ConversionRate("USD", "EUR", 0.85m);
            var cacheResponse = new ConversionRatesCache(new List<ConversionRate> { cachedRate });

            mockCacheStore.Get(Arg.Any<ConversionRatesCacheKey>()).Returns(cacheResponse);
            var foreignExchange = new ForeignExchange(mockCacheStore);

            // Act
            var result = foreignExchange.GetConversionRates();

            // Assert
            Assert.Single(result);

        }
    }
}
