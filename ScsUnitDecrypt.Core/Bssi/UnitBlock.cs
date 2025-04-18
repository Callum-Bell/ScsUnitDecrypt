using System.Collections.Generic;
using System.Text;
using ScsUnitDecrypt.Core.Exceptions;
using ScsUnitDecrypt.Core.Utils;

namespace ScsUnitDecrypt.Core.Bssi
{
    /// <summary>
    ///     Represents all data of a unit, includes the <see cref="LayoutBlock" /> for the unit and gets
    ///     <see cref="UnitAttribute" />s
    ///     when running the <see cref="Parse" /> method
    /// </summary>
    internal class UnitBlock
    {
        private readonly List<UnitAttribute> _attributes = new List<UnitAttribute>();
        private readonly LayoutBlock _layoutBlock;

        private string _unitName;

        internal UnitBlock(LayoutBlock block)
        {
            _layoutBlock = block;
        }

        /// <summary>
        ///     Goes through the binary data and gets all the <see cref="UnitAttribute" />s for the <see cref="_layoutBlock" />
        /// </summary>
        /// <param name="data">Contents of the binary save file</param>
        /// <param name="startOffset">Offset to start reading from</param>
        /// <param name="fileVersion"></param>
        /// <returns>The amount of bytes read</returns>
        /// <exception cref="UnknownUnitAttributeType">Gets thrown when there is a unit we do not know the type of</exception>
        internal int Parse(byte[] data, int startOffset, uint fileVersion)
        {
            var fileOffset = startOffset;
            var (offset, name) = MemoryUtils.ReadUnitName(data, fileOffset);
            _unitName = name;
            fileOffset += offset;

            foreach (var layoutBlockAttributeKeys in _layoutBlock.AttributeKeys)
            {
                try
                {
                    var unitAttribute = UnitAttribute.Create(layoutBlockAttributeKeys.Type, fileVersion,
                        layoutBlockAttributeKeys);
                    fileOffset += unitAttribute.Parse(data, fileOffset);
                    _attributes.Add(unitAttribute);
                }
                catch (UnknownUnitAttributeType)
                {
                    throw new UnknownUnitAttributeType(
                        $"Found unknown unit attribute type {layoutBlockAttributeKeys.Type} in '{_unitName}' for '{layoutBlockAttributeKeys.Name}' @ {fileOffset}.");
                }
            }

            return fileOffset - startOffset;
        }

        /// <summary>
        ///     Gets the unit block in textual format:
        ///     <code>
        /// <see cref="LayoutBlock.ClassName" /> : <see cref="_unitName" /> {
        ///  All <see cref="UnitAttribute" />s from <see cref="_attributes" /> in text format
        /// }
        ///     </code>
        /// </summary>
        internal string GetBlockText()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{_layoutBlock.ClassName} : {_unitName} {{");

            foreach (var attribute in _attributes) sb.Append(attribute.GetLine());

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
