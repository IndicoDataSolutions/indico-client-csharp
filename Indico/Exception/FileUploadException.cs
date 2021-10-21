using System;

namespace Indico.Exception
{


    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    [System.Serializable]
    public class FileUploadException : System.Exception
    {
        public FileUploadException() { }
        public FileUploadException(string message) : base(message) { }
        public FileUploadException(string message, System.Exception inner) : base(message, inner) { }
        protected FileUploadException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
