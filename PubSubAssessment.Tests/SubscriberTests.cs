using PubSubAssessment.Core.Models;
using PubSubAssessment.Core.Subscribers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Tests
{
    public class ConsoleDisplaySubscriberTests
    {
        [Fact]
        public void OnNext_WritesPriceCurrencyAndMovementToWriter()
        {
            var writer = new StringWriter();
            var subscriber = new ConsoleDisplaySubscriber(writer);

            var record = new TransformedRecord(price: 2010m, currency: "USD", timestamp: DateTimeOffset.UtcNow, movement: PriceMovement.Up);

            subscriber.OnNext(record);

            var output = writer.ToString();

            Assert.Contains("2010", output);
            Assert.Contains("USD", output);
            Assert.Contains("Up", output);
        }

        [Fact]
        public void OnNext_CalledTwice_WritesTwoLines()
        {
            var writer = new StringWriter();
            var subscriber = new ConsoleDisplaySubscriber(writer);

            var record = new TransformedRecord(2000m, "USD", DateTimeOffset.UtcNow, PriceMovement.Stable);

            subscriber.OnNext(record);
            subscriber.OnNext(record);

            var lineCount = writer.ToString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            Assert.Equal(2, lineCount);
        }
    }
}
