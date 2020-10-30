namespace Indico.Exception
{
    [System.Serializable]
    public class InputException : RuntimeException
    {
        public InputException() { }
        public InputException(string message) : base(message) { }
        public InputException(string message, System.Exception inner) : base(message, inner) { }
        protected InputException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
