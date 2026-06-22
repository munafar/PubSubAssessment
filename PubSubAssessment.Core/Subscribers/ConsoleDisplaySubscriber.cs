using PubSubAssessment.Core.Models;
using PubSubAssessment.Core.PubSub;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Subscribers
{
    public class ConsoleDisplaySubscriber : ISubscriber<TransformedRecord>
    {
        private readonly TextWriter _writer;

        public ConsoleDisplaySubscriber(TextWriter? writer = null)
        {
            _writer = writer ?? Console.Out;
        }

        public void OnNext(TransformedRecord data)
        {
            _writer.WriteLine(
                $"[{data.Timestamp:HH:mm:ss}] Gold: {data.Price} {data.Currency} ({data.Movement})");
        }
    }
}
