using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.PubSub
{
    public interface ISubscriber<T>
    {
        void OnNext(T data);
    }
}
