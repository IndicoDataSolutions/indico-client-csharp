using System.IO;

namespace Indico.Storage
{
    internal class FileParameter
    {
        public Stream File { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
