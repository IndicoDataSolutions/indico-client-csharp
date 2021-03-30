using System;

namespace Indico.Exception
{
    [Serializable]
    public class NotFoundException : RuntimeException
    {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }
    }
}
