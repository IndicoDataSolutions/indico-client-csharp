using System;
using System.Linq;
using StrawberryShake;

namespace IndicoV2.StrawberryShake.Exceptions
{
    public class GraphQlException : Exception
    {
        public GraphQlException(IOperationResult result) : base(result.Errors.First().Message)
        {
        }
    }
}
