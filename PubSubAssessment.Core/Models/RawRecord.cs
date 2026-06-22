using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Models
{
    public class RawRecord
    {
        public decimal Price { get; set; }
        public decimal PreviousPrice { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public RawRecord()
        {
                
        }

        public RawRecord(decimal price, decimal previousPrice, string currency, DateTimeOffset timestamp)
        {
            Price = price;
            PreviousPrice = previousPrice;
            Currency = currency;
            Timestamp = timestamp;
        }
    }
}
