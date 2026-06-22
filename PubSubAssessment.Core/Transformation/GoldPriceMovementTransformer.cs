using PubSubAssessment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Transformation
{
    public class GoldPriceMovementTransformer : IDataTransformer<RawRecord, TransformedRecord>
    {
        private const decimal StableThresholdPercentage = 0.001m; // 0.1%

        public TransformedRecord Transform(RawRecord input)
        {
            var movement = ClassifyMovement(input.Price, input.PreviousPrice);

            return new TransformedRecord(input.Price, input.Currency, input.Timestamp, movement);
        }

        private static PriceMovement ClassifyMovement(decimal price, decimal previousPrice)
        {
            if (previousPrice == 0)
            {
                return PriceMovement.Stable;
            }

            var change = price - previousPrice;
            var changeRatio = Math.Abs(change) / previousPrice;

            if (changeRatio <= StableThresholdPercentage)
            {
                return PriceMovement.Stable;
            }

            return change > 0 ? PriceMovement.Up : PriceMovement.Down;
        }
    }
}
