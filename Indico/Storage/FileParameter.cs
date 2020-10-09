using System.IO;

namespace Indico.Storage
{
    class FileParameter
    {
        public Stream File { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
