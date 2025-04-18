using System;

namespace ScsUnitDecrypt.Core.Exceptions
{
    internal class UnknownUnitAttributeType : Exception
    {
        public UnknownUnitAttributeType()
        {
        }

        public UnknownUnitAttributeType(string message) : base(message)
        {
        }

        public UnknownUnitAttributeType(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}