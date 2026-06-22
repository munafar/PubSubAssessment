using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.PubSub
{
    public class InMemoryBroker<T> : IPublisher<T>
    {
        private readonly List<ISubscriber<T>> _subscribers = new();

        public void Subscribe(ISubscriber<T> subscriber)
        {
            if (!_subscribers.Contains(subscriber))
            {
                _subscribers.Add(subscriber);
            }
        }

        public void Unsubscribe(ISubscriber<T> subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void Publish(T data)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.OnNext(data);
            }
        }
    }
}
