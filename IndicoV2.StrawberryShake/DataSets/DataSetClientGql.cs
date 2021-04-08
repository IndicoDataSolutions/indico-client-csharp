using IndicoV2.StrawberryShake.DataSets.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.DataSets
{
    public class DataSetClientGql
    {
        private readonly ServiceProvider _services;

        public DataSetClientGql(ServiceProvider services) => _services = services;

        public IAddFilesClient AddFiles() => _services.GetRequiredService<IAddFilesClient>();
    }
}
