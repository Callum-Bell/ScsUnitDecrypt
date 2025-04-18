using System.Security.Cryptography;

namespace ScsUnitDecrypt.Core.Utils
{
    public static class AesDecrypt
    {
        private static readonly byte[] Key =
        {
            0x2a, 0x5f, 0xcb, 0x17, 0x91, 0xd2, 0x2f, 0xb6,
            0x02, 0x45, 0xb3, 0xd8, 0x36, 0x9e, 0xd0, 0xb2,
            0xc2, 0x73, 0x71, 0x56, 0x3f, 0xbf, 0x1f, 0x3c,
            0x9e, 0xdf, 0x6b, 0x11, 0x82, 0x5a, 0x5d, 0x0a
        };

        public static byte[] Decrypt(byte[] encryptedData, byte[] iv)
        {
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                IV = iv,
                KeySize = 0x80,
                BlockSize = 0x80,
                Key = Key
            }.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}