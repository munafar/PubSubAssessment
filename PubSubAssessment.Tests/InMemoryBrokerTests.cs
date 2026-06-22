using PubSubAssessment.Core.PubSub;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Tests
{
    public class InMemoryBrokerTests
    {
        private class RecordingSubscriber : ISubscriber<string>
        {
            public List<string> Received { get; } = new();

            public void OnNext(string data) => Received.Add(data);
        }

        [Fact]
        public void Publish_WithOneSubscriber_DeliversData()
        {
            var broker = new InMemoryBroker<string>();
            var subscriber = new RecordingSubscriber();

            broker.Subscribe(subscriber);
            broker.Publish("gold update 1");

            Assert.Single(subscriber.Received);
            Assert.Equal("gold update 1", subscriber.Received[0]);
        }

        [Fact]
        public void Publish_WithMultipleSubscribers_DeliversToAll()
        {
            var broker = new InMemoryBroker<string>();

            var subscriberA = new RecordingSubscriber();
            var subscriberB = new RecordingSubscriber();
            
            broker.Subscribe(subscriberA);
            broker.Subscribe(subscriberB);
            broker.Publish("gold update 1");

            Assert.Single(subscriberA.Received);
            Assert.Single(subscriberB.Received);
        }

        [Fact]
        public void Publish_AfterUnsubscribe_DoesNotDeliverToThatSubscriber()
        {
            var broker = new InMemoryBroker<string>();
            var subscriber = new RecordingSubscriber();

            broker.Subscribe(subscriber);
            broker.Unsubscribe(subscriber);
            broker.Publish("gold update 1");

            Assert.Empty(subscriber.Received);
        }

        [Fact]
        public void Subscribe_CalledTwiceWithSameSubscriber_DoesNotDuplicateDelivery()
        {
            var broker = new InMemoryBroker<string>();
            var subscriber = new RecordingSubscriber();
            
            broker.Subscribe(subscriber);
            broker.Subscribe(subscriber);
            broker.Publish("gold update 1");

            Assert.Single(subscriber.Received);
        }

        [Fact]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            var broker = new InMemoryBroker<string>();

            var exception = Record.Exception(() => broker.Publish("gold update 1"));

            Assert.Null(exception);
        }
    }
}
