using System;

namespace Indico.Exception
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    [Serializable]
    public class NotFoundException : RuntimeException
    {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }
    }
}
