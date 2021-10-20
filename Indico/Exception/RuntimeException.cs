using System;

namespace Indico.Exception
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    [System.Serializable]
    public class RuntimeException : System.Exception
    {
        public RuntimeException() { }
        public RuntimeException(string message) : base(message) { }
        public RuntimeException(string message, System.Exception inner) : base(message, inner) { }
        protected RuntimeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}