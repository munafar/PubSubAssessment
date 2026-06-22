using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Models
{
    public enum PriceMovement
    {
        Up,
        Down,
        Stable
    }

    public class TransformedRecord
    {
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; set; }
        public PriceMovement Movement { get; set; }

        public TransformedRecord()
        {
        }

        public TransformedRecord(decimal price, string currency, DateTimeOffset timestamp, PriceMovement movement)
        {
            Price = price;
            Currency = currency;
            Timestamp = timestamp;
            Movement = movement;
        }
    }
}
