using System;
using UnityEngine;

namespace BaseDefense.Exceptions {

    public class AttemptDestroyObjectException : UnityException {

        public AttemptDestroyObjectException () { }
        public AttemptDestroyObjectException (string message) : base(message) { }
        public AttemptDestroyObjectException (string message, Exception inner) : base(message, inner) { }

    }

}