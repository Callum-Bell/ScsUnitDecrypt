using System;
using System.Collections.Generic;
using System.Text;

namespace ScsUnitDecrypt.Core.Bssi
{
    /// <summary>
    ///     The layout of a unit block, includes the <see cref="ClassName">class name</see> and the attribute names and
    ///     their value type
    /// </summary>
    internal class LayoutBlock
    {
        internal bool Valid { get; private set; }
        internal uint Id { get; private set; }

        internal string ClassName { get; private set; }

        internal List<UnitAttributeKey> AttributeKeys { get; } = new List<UnitAttributeKey>();

        /// <summary>
        ///     Goes through the binary data and gets all the layout of all the units
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startOffset"></param>
        /// <returns>The amount of bytes read</returns>
        internal int Parse(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            Valid = data[fileOffset] == 1;
            fileOffset += 0x01; // 0x01(valid)

            if (!Valid) return fileOffset - startOffset;

            Id = BitConverter.ToUInt32(data, fileOffset);
            var classNameLength = BitConverter.ToInt32(data, fileOffset += 0x04); // 0x04(Id)
            ClassName = Encoding.UTF8.GetString(data, fileOffset += 0x04, classNameLength); // 0x04(classNameLength)
            fileOffset += classNameLength;

            while (true)
            {
                var valueType = BitConverter.ToUInt32(data, fileOffset);
                fileOffset += 0x04; // 0x04(valueType)
                if (valueType == 0) break;

                var attributeNameLength = BitConverter.ToUInt32(data, fileOffset);
                var attribute = new UnitAttributeKey((UnitAttributeType) valueType,
                    Encoding.UTF8.GetString(data, fileOffset += 0x04,
                        (int) attributeNameLength)); // 0x04(attributeNameLength)
                fileOffset += (int) attributeNameLength;

                if (valueType == 0x37) // ordinalString
                {
                    var totalCount = BitConverter.ToUInt32(data, fileOffset);
                    fileOffset += 0x04; // 0x04(totalCount)
                    for (var i = 0; i < totalCount; i++)
                    {
                        var index = BitConverter.ToUInt32(data, fileOffset);
                        var textLength = BitConverter.ToUInt32(data, fileOffset += 0x04); // 0x04(index)
                        attribute.AddOrdinalString(index,
                            Encoding.UTF8.GetString(data, fileOffset += 0x04, (int) textLength)); // 0x04(textLength)
                        fileOffset += (int) textLength;
                    }
                }

                AttributeKeys.Add(attribute);
            }

            return fileOffset - startOffset;
        }
    }
}