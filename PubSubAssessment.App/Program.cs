using System;
using PubSubAssessment.Core.Models;
using PubSubAssessment.Core.PubSub;
using PubSubAssessment.Core.Subscribers;
using PubSubAssessment.Core.Transformation;

namespace PubSubAssessment.ConsoleApp
{
    public class Program
    {
        private const int TickCount = 8;
        private const decimal StartingPrice = 2000m;
        private const decimal MaxFluctuation = 5m;

        public static void Main(string[] args)
        {
            var broker = new InMemoryBroker<TransformedRecord>();
            var transformer = new GoldPriceMovementTransformer();

            broker.Subscribe(new ConsoleDisplaySubscriber());
            broker.Subscribe(new MovementAlertSubscriber());

            var random = new Random();
            var previousPrice = StartingPrice;

            for (var i = 0; i < TickCount; i++)
            {
                var fluctuation = (decimal)(random.NextDouble() * 2 - 1) * MaxFluctuation;
                var currentPrice = Math.Round(previousPrice + fluctuation, 2);

                var raw = new RawRecord(price: currentPrice, previousPrice: previousPrice, currency: "USD", timestamp: DateTimeOffset.UtcNow);

                var transformed = transformer.Transform(raw);
                broker.Publish(transformed);

                previousPrice = currentPrice;
            }
        }
    }
}