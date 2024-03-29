﻿using System.IO;

namespace IndicoV2.Storage
{
    internal class FileParameter
    {
        public Stream File { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
