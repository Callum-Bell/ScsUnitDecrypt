using System;

namespace ScsUnitDecrypt.Core.Exceptions
{
    internal class LayoutBlockNotFound : Exception
    {
        public LayoutBlockNotFound()
        {
        }

        public LayoutBlockNotFound(string message) : base(message)
        {
        }

        public LayoutBlockNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}