using PubSubAssessment.Core.Models;
using PubSubAssessment.Core.Transformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Tests
{
    public class GoldPriceMovementTransformerTests
    {
        private readonly GoldPriceMovementTransformer _transformer = new();

        [Fact]
        public void Transform_WhenPriceIncreasesAboveThreshold_ReturnsUp()
        {
            var input = new RawRecord(price: 2010m, previousPrice: 2000m, currency: "USD", timestamp: DateTimeOffset.UtcNow);

            var result = _transformer.Transform(input);

            Assert.Equal(PriceMovement.Up, result.Movement);
        }

        [Fact]
        public void Transform_WhenPriceDecreasesAboveThreshold_ReturnsDown()
        {
            var input = new RawRecord(price: 1990m, previousPrice: 2000m, currency: "USD", timestamp: DateTimeOffset.UtcNow);

            var result = _transformer.Transform(input);

            Assert.Equal(PriceMovement.Down, result.Movement);
        }

        [Fact]
        public void Transform_WhenPriceChangeWithinThreshold_ReturnsStable()
        {
            // 2000 * 0.1% = 2.00, so a 1.00 change should stay within the Stable band
            var input = new RawRecord(price: 2001m, previousPrice: 2000m, currency: "USD", timestamp: DateTimeOffset.UtcNow);

            var result = _transformer.Transform(input);

            Assert.Equal(PriceMovement.Stable, result.Movement);
        }

        [Fact]
        public void Transform_WhenPriceUnchanged_ReturnsStable()
        {
            var input = new RawRecord(price: 2000m, previousPrice: 2000m, currency: "USD", timestamp: DateTimeOffset.UtcNow);

            var result = _transformer.Transform(input);

            Assert.Equal(PriceMovement.Stable, result.Movement);
        }

        [Fact]
        public void Transform_WhenPreviousPriceIsZero_ReturnsStableWithoutThrowing()
        {
            var input = new RawRecord( price: 2000m, previousPrice: 0m, currency: "USD", timestamp: DateTimeOffset.UtcNow);

            var result = _transformer.Transform(input);

            Assert.Equal(PriceMovement.Stable, result.Movement);
        }

        [Fact]
        public void Transform_PreservesPriceCurrencyAndTimestampFromInput()
        {
            var timestamp = new DateTimeOffset(2026, 6, 22, 9, 0, 0, TimeSpan.Zero);

            var input = new RawRecord(price: 2050m, previousPrice: 2000m, currency: "GBP", timestamp: timestamp);

            var result = _transformer.Transform(input);

            Assert.Equal(2050m, result.Price);
            Assert.Equal("GBP", result.Currency);
            Assert.Equal(timestamp, result.Timestamp);
        }
    }
}
