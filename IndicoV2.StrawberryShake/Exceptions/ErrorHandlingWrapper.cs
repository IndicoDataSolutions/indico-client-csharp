using System;
using System.Linq;
using System.Threading.Tasks;
using StrawberryShake;

namespace IndicoV2.StrawberryShake.Exceptions
{
    public abstract class ErrorHandlingWrapper<T>
    {
        protected readonly T _inner;

        protected ErrorHandlingWrapper(T inner) => _inner = inner;

        protected async Task<TResult> ExecuteAsync<TResult>(Func<Task<IOperationResult<TResult>>> executeAsync)
            where TResult : class
        {
            var result = await executeAsync();

            if (result.Errors != null && result.Errors.Any())
            {
                throw new GraphQlException(result);
            }

            return result.Data;
        }
    }
}
