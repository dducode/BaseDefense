using System;

namespace BaseDefense.Exceptions
{
    public class AttemptDestroyObjectException : Exception
    {
        public AttemptDestroyObjectException() {}
        public AttemptDestroyObjectException(string message) : base(message) {}
        public AttemptDestroyObjectException(string message, Exception inner) : base(message, inner) {}
    }
}