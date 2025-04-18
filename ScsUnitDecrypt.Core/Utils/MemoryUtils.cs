using System;
using ScsUnitDecrypt.Core.Common;

namespace ScsUnitDecrypt.Core.Utils
{
    public static class MemoryUtils
    {
        internal static (int, string) ReadUnitName(byte[] data, int startOffset)
        {
            var fileOffset = startOffset;
            var idCount = data[fileOffset];
            fileOffset += 0x01;
            if (idCount == 0) return (0x01, "");

            var isToken = true;

            if (idCount == 255)
            {
                idCount = 1;
                isToken = false;
            }

            var val = "";
            for (var i = 0; i < idCount; i++)
            {
                val += val == "" ? "" : ".";
                var part = BitConverter.ToUInt64(data, fileOffset);
                fileOffset += 0x08; // 0x08(part)
                val += isToken ? ScsToken.TokenToString(part) : SiiUtils.FormatMemoryAddress(part);
            }

            return (fileOffset - startOffset, val);
        }

        internal static byte[] Decrypt3Nk(byte[] src) // from quickbms scsgames.bms script
        {
            if (src.Length < 0x05 || (src[0] != 0x33 && src[1] != 0x6E && src[2] != 0x4B)) return src;
            var decrypted = new byte[src.Length - 6];
            var key = src[5];

            for (var i = 6; i < src.Length; i++)
            {
                decrypted[i - 6] = (byte) ((((key << 2) ^ key ^ 0xff) << 3) ^ key ^ src[i]);
                key++;
            }

            return decrypted;
        }
    }
}
