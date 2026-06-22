using PubSubAssessment.Core.Models;
using PubSubAssessment.Core.PubSub;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Subscribers
{
    public class MovementAlertSubscriber : ISubscriber<TransformedRecord>
    {
        private readonly TextWriter _writer;

        public MovementAlertSubscriber(TextWriter? writer = null)
        {
            _writer = writer ?? Console.Out;
        }

        public void OnNext(TransformedRecord data)
        {
            if (data.Movement == PriceMovement.Stable)
            {
                return;
            }

            _writer.WriteLine($"  ALERT: gold moved from {data.Movement} to {data.Price} {data.Currency}");
        }
    }
}
