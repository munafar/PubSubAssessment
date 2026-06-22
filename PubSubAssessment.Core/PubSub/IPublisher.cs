using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.PubSub
{
    public interface IPublisher<T>
    {
        void Subscribe(ISubscriber<T> subscriber);
        void Unsubscribe(ISubscriber<T> subscriber);
        void Publish(T data);
    }
}
