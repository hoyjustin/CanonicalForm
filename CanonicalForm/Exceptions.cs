using System;
using System.Runtime.Serialization;

namespace CanonicalFormExceptions
{
    [Serializable]
    public class InvalidEquationException : Exception
    {
        public InvalidEquationException()
        {
        }

        public InvalidEquationException(string message) : base(message)
        {
        }

        public InvalidEquationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidEquationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidSummandOperationException : Exception
    {
        public InvalidSummandOperationException()
        {
        }

        public InvalidSummandOperationException(string message) : base(message)
        {
        }

        public InvalidSummandOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSummandOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}