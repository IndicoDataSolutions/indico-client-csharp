namespace Indico.Exception
{
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
