using System;

namespace ScsUnitDecrypt.Core.Exceptions
{
    internal class IncompatibleFileTypeException : Exception
    {
        public IncompatibleFileTypeException()
        {
        }

        public IncompatibleFileTypeException(string message) : base(message)
        {
        }

        public IncompatibleFileTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}