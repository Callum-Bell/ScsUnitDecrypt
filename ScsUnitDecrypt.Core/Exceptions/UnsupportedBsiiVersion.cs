using System;

namespace ScsUnitDecrypt.Core.Exceptions
{
    internal class UnsupportedBsiiVersion : Exception
    {
        public UnsupportedBsiiVersion()
        {
        }

        public UnsupportedBsiiVersion(string message) : base(message)
        {
        }

        public UnsupportedBsiiVersion(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}