using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ScsUnitDecrypt.Core.Common;
using ScsUnitDecrypt.Core.Exceptions;
using ScsUnitDecrypt.Core.Utils;

namespace ScsUnitDecrypt.Core.Bssi
{
    internal abstract class UnitAttribute
    {
        protected readonly UnitAttributeKey AttributeKey;

        protected UnitAttribute(UnitAttributeKey attributeKey)
        {
            AttributeKey = attributeKey;
        }

        protected string GetKeyName()
        {
            return AttributeKey.Name;
        }

        internal static UnitAttribute Create(UnitAttributeType type, uint fileVersion,
            UnitAttributeKey attributeKey)
        {
            switch (type)
            {
                case UnitAttributeType.String: return new UnitString(attributeKey);
                case UnitAttributeType.StringArray: return new UnitStringArray(attributeKey);
                case UnitAttributeType.Token: return new UnitToken(attributeKey);
                case UnitAttributeType.TokenArray: return new UnitTokenArray(attributeKey);
                case UnitAttributeType.Float: return new UnitFloat(attributeKey);
                case UnitAttributeType.FloatArray: return new UnitFloatArray(attributeKey);
                case UnitAttributeType.Float2: return new UnitFloat2(attributeKey);
                case UnitAttributeType.Float3: return new UnitFloat3(attributeKey);
                case UnitAttributeType.Float3Array: return new UnitFloat3Array(attributeKey);
                case UnitAttributeType.Fixed3: return new UnitInt3(attributeKey);
                case UnitAttributeType.Fixed3Array: return new UnitInt3Array(attributeKey);
                case UnitAttributeType.QuatArray:
                case UnitAttributeType.Float4Array: return new UnitFloat4Array(attributeKey);
                case UnitAttributeType.Placement:
                    if (fileVersion == 1)
                        return new UnitFloat7(attributeKey);
                    return new UnitFloat8(attributeKey);
                case UnitAttributeType.PlacementArray:
                    if (fileVersion == 1)
                        return new UnitFloat7Array(attributeKey);
                    return new UnitFloat8Array(attributeKey);
                case UnitAttributeType.Int32: return new UnitInt32(attributeKey);
                case UnitAttributeType.Int32Array_2:
                case UnitAttributeType.Int32Array: return new UnitInt32Array(attributeKey);
                case UnitAttributeType.UInt32_2:
                case UnitAttributeType.UInt32: return new UnitUInt32(attributeKey);
                case UnitAttributeType.UInt32Array_2:
                case UnitAttributeType.UInt32Array: return new UnitUInt32Array(attributeKey);
                case UnitAttributeType.UInt16: return new UnitUInt16(attributeKey);
                case UnitAttributeType.UInt16Array: return new UnitUInt16Array(attributeKey);
                case UnitAttributeType.Int64: return new UnitInt64(attributeKey);
                case UnitAttributeType.Int64Array: return new UnitInt64Array(attributeKey);
                case UnitAttributeType.UInt64: return new UnitUInt64(attributeKey);
                case UnitAttributeType.UInt64Array: return new UnitUInt64Array(attributeKey);
                case UnitAttributeType.Bool: return new UnitBool(attributeKey);
                case UnitAttributeType.BoolArray: return new UnitBoolArray(attributeKey);
                case UnitAttributeType.OrdinalString: return new UnitOrdinalString(attributeKey);
                case UnitAttributeType.Id039:
                case UnitAttributeType.Id03B:
                case UnitAttributeType.Id03D:
                    return new UnitId(attributeKey);
                case UnitAttributeType.IdArray03A:
                case UnitAttributeType.IdArray03C:
                    return new UnitIdArray(attributeKey);
                case UnitAttributeType.Int2: return new UnitInt2(attributeKey);
                case UnitAttributeType.Quat:
                    return new UnitQuat(attributeKey);
                default:
                    throw new UnknownUnitAttributeType();
            }
        }

        protected string GetAttributeType()
        {
            return $"{AttributeKey.Type}";
        }

        internal abstract string GetLine();

        internal virtual string GetComment()
        {
            return BsiiReader.Options.HasFlag(DecodeOptions.TypeComments) ? $" # TYPE = {GetAttributeType()}" : "";
        }

        internal abstract int Parse(byte[] data, int startOffset);
    }

    internal class UnitString : UnitAttribute
    {
        private string _value;

        internal UnitString(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {SiiUtils.StringToSiiFormat(_value)}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var textLength = BitConverter.ToInt32(data, fileOffset);
            _value = Encoding.UTF8.GetString(data, fileOffset += 0x04, textLength); // 0x04(textLength)
            return fileOffset + textLength - startOffset;
        }
    }

    internal class UnitStringArray : UnitAttribute
    {
        private readonly List<string> _values = new List<string>();

        internal UnitStringArray(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {SiiUtils.StringToSiiFormat(_values[i])}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var textCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(textCount)
            for (var i = 0; i < textCount; i++)
            {
                var textLength = BitConverter.ToInt32(data, fileOffset);
                _values.Add(Encoding.UTF8.GetString(data, fileOffset += 0x04, textLength)); // 0x04(textLength)
                fileOffset += textLength;
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitToken : UnitAttribute
    {
        private ulong _token;

        internal UnitToken(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {SiiUtils.StringToSiiFormat(ScsToken.TokenToString(_token))}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _token = BitConverter.ToUInt64(data, startOffset);
            return 0x08; // 0x08(_token)
        }
    }

    internal class UnitTokenArray : UnitAttribute
    {
        private readonly List<ulong> _tokens = new List<ulong>();

        internal UnitTokenArray(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_tokens.Count}{GetComment()}");
            for (var i = 0; i < _tokens.Count; i++)
            {
                sb.AppendLine(
                    $" {GetKeyName()}[{i}]: {SiiUtils.StringToSiiFormat(ScsToken.TokenToString(_tokens[i]))}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var tokenCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(tokenCount)
            for (var i = 0; i < tokenCount; i++)
            {
                _tokens.Add(BitConverter.ToUInt64(data, fileOffset));
                fileOffset += 0x08; // 0x08(token)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitFloat : UnitAttribute
    {
        private float _value;

        internal UnitFloat(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                ? $" # {_value.ToString(CultureInfo.CreateSpecificCulture("en-US"))}f"
                : "";
            return
                $" {GetKeyName()}: {SiiUtils.FloatToSiiFormat(_value)}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToSingle(data, startOffset);
            return 0x04; // 0x04(_value)
        }
    }

    internal class UnitFloatArray : UnitAttribute
    {
        private readonly List<float> _values = new List<float>();

        internal UnitFloatArray(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                    ? $" # {_values[i].ToString(CultureInfo.CreateSpecificCulture("en-US"))}f"
                    : "";
                sb.AppendLine($" {GetKeyName()}[{i}]: {SiiUtils.FloatToSiiFormat(_values[i])}{comment}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToSingle(data, fileOffset));
                fileOffset += 0x04; // 0x04(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitFloat2 : UnitAttribute
    {
        private Float2 _value;

        internal UnitFloat2(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments) ? $" # {_value.GetComment()}" : "";
            return $" {GetKeyName()}: {_value}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = new Float2
            {
                X = BitConverter.ToSingle(data, startOffset),
                Y = BitConverter.ToSingle(data, startOffset + 4)
            };
            return 0x08; // 0x08(X, Y)
        }
    }

    internal class UnitFloat3 : UnitAttribute
    {
        private Float3 _value;

        internal UnitFloat3(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments) ? $" # {_value.GetComment()}" : "";
            return $" {GetKeyName()}: {_value}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = new Float3
            {
                X = BitConverter.ToSingle(data, startOffset),
                Y = BitConverter.ToSingle(data, startOffset + 0x04),
                Z = BitConverter.ToSingle(data, startOffset + 0x08)
            };
            return 0x0C; // 0x0C(X, Y, Z)
        }
    }

    internal class UnitFloat3Array : UnitAttribute
    {
        private readonly List<Float3> _values = new List<Float3>();

        internal UnitFloat3Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                    ? $" # {_values[i].GetComment()}"
                    : "";
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}{comment}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(new Float3
                {
                    X = BitConverter.ToSingle(data, fileOffset),
                    Y = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(X)
                    Z = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(Y)
                });
                fileOffset += 0x04; // 0x04(Z)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitInt3 : UnitAttribute
    {
        private Int3 _value;

        internal UnitInt3(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {_value}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = new Int3
            {
                X = BitConverter.ToInt32(data, startOffset),
                Y = BitConverter.ToInt32(data, startOffset + 0x04),
                Z = BitConverter.ToInt32(data, startOffset + 0x08)
            };
            return 0x0C; // 0x0C(X, Y, Z)
        }
    }

    internal class UnitInt3Array : UnitAttribute
    {
        private readonly List<Int3> _values = new List<Int3>();

        internal UnitInt3Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(new Int3
                {
                    X = BitConverter.ToInt32(data, fileOffset),
                    Y = BitConverter.ToInt32(data, fileOffset += 0x04), // 0x04(X)
                    Z = BitConverter.ToInt32(data, fileOffset += 0x04) // 0x04(Y)
                });
                fileOffset += 0x04; // 0x04(Z)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitFloat4Array : UnitAttribute
    {
        private readonly List<Float4> _values = new List<Float4>();

        internal UnitFloat4Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                    ? $" # {_values[i].GetComment()}"
                    : "";
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}{comment}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(new Float4
                {
                    X = BitConverter.ToSingle(data, fileOffset),
                    Y = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(X)
                    Z = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(Y)
                    W = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(Z)
                });
                fileOffset += 0x04; // 0x04(W)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitFloat7 : UnitAttribute
    {
        private Float7 _value;

        internal UnitFloat7(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments) ? $" # {_value.GetComment()}" : "";
            return $" {GetKeyName()}: {_value}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            _value = new Float7
            {
                F1 = BitConverter.ToSingle(data, fileOffset),
                F2 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F1)
                F3 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F2)
                F4 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F3)
                F5 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F4)
                F6 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F5)
                F7 = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(F6)
            };
            return fileOffset + 0x04 - startOffset; // 0x04(F7)
        }
    }

    internal class UnitFloat8 : UnitAttribute
    {
        private Float8 _value;

        internal UnitFloat8(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments) ? $" # {_value.GetComment()}" : "";
            return $" {GetKeyName()}: {_value}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            _value = new Float8
            {
                F1 = BitConverter.ToSingle(data, fileOffset),
                F2 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F1)
                F3 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F2)
                F4 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F3)
                F5 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F4)
                F6 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F5)
                F7 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F6)
                F8 = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(F7)
            };
            return fileOffset + 0x04 - startOffset; // 0x04(F8)
        }
    }

    internal class UnitFloat7Array : UnitAttribute
    {
        private readonly List<Float7> _values = new List<Float7>();

        internal UnitFloat7Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                    ? $" # {_values[i].GetComment()}"
                    : "";
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}{comment}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(new Float7
                {
                    F1 = BitConverter.ToSingle(data, fileOffset),
                    F2 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F1)
                    F3 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F2)
                    F4 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F3)
                    F5 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F4)
                    F6 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F5)
                    F7 = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(F6)
                });
                fileOffset += 0x04; // 0x04(F7)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitFloat8Array : UnitAttribute
    {
        private readonly List<Float8> _values = new List<Float8>();

        internal UnitFloat8Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                var comment = BsiiReader.Options.HasFlag(DecodeOptions.FloatComments)
                    ? $" # {_values[i].GetComment()}"
                    : "";
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}{comment}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(new Float8
                {
                    F1 = BitConverter.ToSingle(data, fileOffset),
                    F2 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F1)
                    F3 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F2)
                    F4 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F3)
                    F5 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F4)
                    F6 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F5)
                    F7 = BitConverter.ToSingle(data, fileOffset += 0x04), // 0x04(F6)
                    F8 = BitConverter.ToSingle(data, fileOffset += 0x04) // 0x04(F7)
                });
                fileOffset += 0x04; // 0x04(F8)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitInt32 : UnitAttribute
    {
        private int _value;

        internal UnitInt32(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {_value}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToInt32(data, startOffset);
            return 0x04; // 0x04(_value)
        }
    }

    internal class UnitInt32Array : UnitAttribute
    {
        private readonly List<int> _values = new List<int>();

        internal UnitInt32Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToInt32(data, fileOffset));
                fileOffset += 0x04; // 0x04(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitUInt32 : UnitAttribute
    {
        private uint _value;

        internal UnitUInt32(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {(_value == uint.MaxValue ? "nil" : $"{_value}")}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToUInt32(data, startOffset);
            return 0x04; // 0x04(_value)
        }
    }

    internal class UnitUInt32Array : UnitAttribute
    {
        private readonly List<uint> _values = new List<uint>();

        internal UnitUInt32Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {(_values[i] == uint.MaxValue ? "nil" : $"{_values[i]}")}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToUInt32(data, fileOffset));
                fileOffset += 0x04; // 0x04(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitUInt16 : UnitAttribute
    {
        private ushort _value;

        internal UnitUInt16(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {(_value == ushort.MaxValue ? "nil" : $"{_value}")}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToUInt16(data, startOffset);
            return 0x02; // 0x02(_value)
        }
    }

    internal class UnitUInt16Array : UnitAttribute
    {
        private readonly List<ushort> _values = new List<ushort>();

        internal UnitUInt16Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
            {
                sb.AppendLine(
                    $" {GetKeyName()}[{i}]: {(_values[i] == ushort.MaxValue ? "nil" : $"{_values[i]}")}");
            }

            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToUInt16(data, fileOffset));
                fileOffset += 0x02; // 0x02(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitInt64 : UnitAttribute
    {
        private long _value;

        internal UnitInt64(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {_value}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToInt64(data, startOffset);
            return 0x08; // 0x08(_value)
        }
    }

    internal class UnitInt64Array : UnitAttribute
    {
        private readonly List<long> _values = new List<long>();

        internal UnitInt64Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {_values[i]}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToInt64(data, fileOffset));
                fileOffset += 0x08; // 0x08(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitUInt64 : UnitAttribute
    {
        private ulong _value;

        internal UnitUInt64(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {(_value == ulong.MaxValue ? "nil" : $"{_value}")}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToUInt64(data, startOffset);
            return 0x08; // 0x08(_value)
        }
    }

    internal class UnitUInt64Array : UnitAttribute
    {
        private readonly List<ulong> _values = new List<ulong>();

        internal UnitUInt64Array(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {(_values[i] == ulong.MaxValue ? "nil" : $"{_values[i]}")}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToUInt64(data, fileOffset));
                fileOffset += 0x08; // 0x08(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitBool : UnitAttribute
    {
        private bool _value;

        internal UnitBool(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {(_value ? "true" : "false")}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = BitConverter.ToBoolean(data, startOffset);
            return 0x01; // 0x01(_value)
        }
    }

    internal class UnitBoolArray : UnitAttribute
    {
        private readonly List<bool> _values = new List<bool>();

        internal UnitBoolArray(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_values.Count}{GetComment()}");
            for (var i = 0; i < _values.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {(_values[i] ? "true" : "false")}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var valueCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(valueCount)
            for (var i = 0; i < valueCount; i++)
            {
                _values.Add(BitConverter.ToBoolean(data, fileOffset));
                fileOffset += 0x01; // 0x01(value)
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitOrdinalString : UnitAttribute
    {
        private uint _index;

        internal UnitOrdinalString(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var comment = BsiiReader.Options.HasFlag(DecodeOptions.OrdinalStringOptions)
                ? $" # Available Options = ({string.Join(", ", AttributeKey.GetOrdinalStrings())})"
                : "";
            return $" {GetKeyName()}: {AttributeKey.GetOrdinalString(_index)}{comment}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _index = BitConverter.ToUInt32(data, startOffset);
            return 0x04; // 0x04(_index)
        }
    }

    internal class UnitId : UnitAttribute
    {
        private string _id;

        internal UnitId(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {(_id == "" ? "null" : _id)}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var (offset, val) = MemoryUtils.ReadUnitName(data, startOffset);
            _id = val;
            return offset;
        }
    }

    internal class UnitIdArray : UnitAttribute
    {
        private readonly List<string> _ids = new List<string>();

        internal UnitIdArray(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine($" {GetKeyName()}: {_ids.Count}{GetComment()}");
            for (var i = 0; i < _ids.Count; i++)
                sb.AppendLine($" {GetKeyName()}[{i}]: {(_ids[i] == "" ? "null" : _ids[i])}");
            return sb.ToString();
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;

            var idsCount = BitConverter.ToInt32(data, fileOffset);
            fileOffset += 0x04; // 0x04(idsCount)
            for (var x = 0; x < idsCount; x++)
            {
                var (offset, val) = MemoryUtils.ReadUnitName(data, fileOffset);
                fileOffset += offset;
                _ids.Add(val);
            }

            return fileOffset - startOffset;
        }
    }

    internal class UnitInt2 : UnitAttribute
    {
        private Int2 _value;

        internal UnitInt2(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {_value}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = new Int2
            {
                X = BitConverter.ToInt32(data, startOffset),
                Y = BitConverter.ToInt32(data, startOffset + 0x04)
            };
            return 0x08; // 0x08(X, Y)
        }
    }

    internal class UnitQuat : UnitAttribute
    {
        private Quaternion _value;

        internal UnitQuat(UnitAttributeKey attributeKey) : base(attributeKey)
        {
        }

        internal override string GetLine()
        {
            return $" {GetKeyName()}: {_value}{GetComment()}\r\n";
        }

        internal override int Parse(byte[] data, int startOffset)
        {
            _value = new Quaternion
            {
                X = BitConverter.ToSingle(data, startOffset),
                Y = BitConverter.ToSingle(data, startOffset + 0x04),
                Z = BitConverter.ToSingle(data, startOffset + 0x08),
                W = BitConverter.ToSingle(data, startOffset + 0x0C)
            };
            return 0x10;
        }
    }
}
