using IndicoV2.Extensions.Jobs;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static IJobAwaiter JobAwaiter(this IndicoClient indicoClient) => new JobAwaiter(indicoClient.Jobs());
    }
}
