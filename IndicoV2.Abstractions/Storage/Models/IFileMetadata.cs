namespace IndicoV2.Storage.Models
{
    public interface IFileMetadata
    {
        string Name { get; }
        string Path { get; }
        UploadType UploadType { get; }
    }
}