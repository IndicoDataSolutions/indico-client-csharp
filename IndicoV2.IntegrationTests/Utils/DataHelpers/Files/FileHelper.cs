using System.IO;
using System.Threading.Tasks;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Files
{
    public class FileHelper
    {
        public string GetSampleFilePath() => "./Utils/DataHelpers/Files/workflow-sample.pdf";

        public async Task<Stream> GetSampleFileStream() => File.OpenRead(GetSampleFilePath());
    }
}
