using System;

namespace BaseDefense.Exceptions
{
    public class RepeatedDestructionObjectException : Exception
    {
        public RepeatedDestructionObjectException() {}
        public RepeatedDestructionObjectException(string message) : base(message) {}
        public RepeatedDestructionObjectException(string message, Exception inner) : base(message, inner) {}
    }
}