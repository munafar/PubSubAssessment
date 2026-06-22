using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubAssessment.Core.Transformation
{
    public interface IDataTransformer<TIn, TOut>
    {
        TOut Transform(TIn input);
    }
}
