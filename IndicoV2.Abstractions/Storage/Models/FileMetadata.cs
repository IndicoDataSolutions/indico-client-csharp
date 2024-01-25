using Newtonsoft.Json;

namespace IndicoV2.Storage.Models
{
    public class FileMetadata : IFileMetadata
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string Path { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UploadType UploadType { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int Size { get; internal set; }
    }
}