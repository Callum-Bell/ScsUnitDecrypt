using System;

namespace ScsUnitDecrypt.Core.Common
{
    [Flags]
    public enum DecodeOptions : ushort
    {
        None = 0,
        FloatComments = 1 << 0,
        OrdinalStringOptions = 1 << 1,
        TypeComments = 1 << 2
    }
}