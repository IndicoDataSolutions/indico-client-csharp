using System.IO;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.Files
{
    public class FileHelper
    {
        public string GetSampleFilePath() => "./Utils/DataHelpers/Files/workflow-sample.pdf";
        public string GetSampleCsvPath() => "./Utils/DataHelpers/Files/TooSmall.csv";

        public Stream GetSampleFileStream() => File.OpenRead(GetSampleFilePath());
    }
}
