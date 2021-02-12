using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
    public static class ReturnsExtensionsJson
    {
        public static IReturnsResult<TMock> ReturnsJsonStream<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, string json)
            where TMock : class
            where TResult : Stream
            =>
                mock.ReturnsAsync((TResult) (Stream)new MemoryStream(Encoding.UTF8.GetBytes(json)));
    }
}
