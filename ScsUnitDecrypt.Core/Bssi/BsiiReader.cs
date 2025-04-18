using System;
using System.Collections.Generic;
using System.Text;
using ScsUnitDecrypt.Core.Common;
using ScsUnitDecrypt.Core.Exceptions;

namespace ScsUnitDecrypt.Core.Bssi
{
    public class BsiiReader
    {
        internal static DecodeOptions Options;
        private readonly byte[] _data;

        private readonly Dictionary<uint, LayoutBlock> _layoutBlocks = new Dictionary<uint, LayoutBlock>();

        private readonly List<UnitBlock> _unitBlocks = new List<UnitBlock>();

        public BsiiReader(byte[] data, DecodeOptions decodeOptions)
        {
            _data = data;
            Options = decodeOptions;
        }

        /// <summary>
        ///     <para>
        ///         Goes through the whole binary Sii file, reading the layout of the units,
        ///         and then the values that correspond the the layout.
        ///     </para>
        ///     That data can then be retrieved with <see cref="GetData" />
        /// </summary>
        /// <exception cref="UnsupportedBsiiVersion"></exception>
        /// <exception cref="LayoutBlockNotFound">Thrown when a data block references a non existing layout block</exception>
        private void Parse()
        {
            var fileOffset = 0x04; // 0x04(magic)
            var fileVersion = BitConverter.ToUInt32(_data, fileOffset);

            if (fileVersion > 3)
            {
                throw new UnsupportedBsiiVersion(
                    $"Binary sii version {fileVersion} is not supported, only 1-3 is");
            }

            fileOffset += 0x04; // 0x04(version)

            while (true)
            {
                var blockType = BitConverter.ToUInt32(_data, fileOffset); // or value block id
                var blockStartOffset = fileOffset;

                fileOffset += 0x04; // 0x04(blockType)
                if (blockType == 0) // layout block
                {
                    var lb = new LayoutBlock();
                    var blockSize = lb.Parse(_data, fileOffset);
                    fileOffset += blockSize;
                    if (!lb.Valid) break;
                    _layoutBlocks.Add(lb.Id, lb);
                }
                else
                {
                    if (!_layoutBlocks.ContainsKey(blockType))
                    {
                        throw new LayoutBlockNotFound(
                            $"Found a value block (ID: {blockType} @ {blockStartOffset}) that has no corresponding layout block.");
                    }

                    var layoutBlock = _layoutBlocks[blockType];

                    var unitBlock = new UnitBlock(layoutBlock);
                    fileOffset += unitBlock.Parse(_data, fileOffset, fileVersion);
                    _unitBlocks.Add(unitBlock);
                }
            }
        }

        /// <summary>
        ///     Gets the data of the file
        /// </summary>
        /// <returns>Textual file contents as a byte array</returns>
        public byte[] GetData()
        {
            Parse();

            var sb = new StringBuilder("SiiNunit\r\n");
            sb.AppendLine("{");

            foreach (var unitBlock in _unitBlocks) sb.AppendLine(unitBlock.GetBlockText());

            sb.AppendLine("}");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
