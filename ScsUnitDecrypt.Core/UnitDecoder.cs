using System;
using System.IO;
using System.Text;
using Ionic.Zlib;
using ScsUnitDecrypt.Core.Bssi;
using ScsUnitDecrypt.Core.Common;
using ScsUnitDecrypt.Core.Exceptions;
using ScsUnitDecrypt.Core.Utils;

namespace ScsUnitDecrypt.Core
{
    public class UnitDecoder
    {
        private readonly DecodeOptions _decodeOptions;

        public UnitDecoder(DecodeOptions options = DecodeOptions.None)
        {
            _decodeOptions = options;
        }

        /// <summary>
        ///     Reads all bytes of the file for the given file path
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        private static byte[] ReadFileData(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Could not find file '{filePath}'");

            return File.ReadAllBytes(filePath);
        }

        public static FileType GetFileType(uint magic)
        {
            switch (magic)
            {
                case 0x49495342:
                    return FileType.Bsii;
                case 0x43736353:
                    return FileType.ScsC;
                case 0x4E696953:
                    return FileType.Text;
                case 0x14B6E33:
                    return FileType.ThreeNk;
                default:
                    return FileType.Unknown;
            }
        }

        private byte[] ReadBsiiData(byte[] fileData)
        {
            var bssiReader = new BsiiReader(fileData, _decodeOptions);
            return bssiReader.GetData();
        }

        /// <summary>
        ///     Goes through and reads/decodes the data until it returns the text contents.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string DecodeToString(string filePath)
        {
            return Encoding.UTF8.GetString(Parse(filePath));
        }


        private byte[] Parse(string filePath)
        {
            var fileData = ReadFileData(filePath);
            while (true)
            {
                var magic = BitConverter.ToUInt32(fileData, 0);
                switch (GetFileType(magic))
                {
                    case FileType.ScsC:
                        fileData = RemoveEncryption(fileData);
                        continue;
                    case FileType.Bsii:
                        fileData = ReadBsiiData(fileData);
                        continue;
                    case FileType.Text:
                        return fileData;
                    case FileType.ThreeNk:
                        return MemoryUtils.Decrypt3Nk(fileData);
                    case FileType.Unknown:
                    default:
                        throw new IncompatibleFileTypeException($"Incompatible file type found (0x{magic:X})");
                }
            }
        }

        /// <summary>
        ///     Goes through and reads/decodes the data until it writes the text contents to a file,
        ///     if path is not given in <paramref name="fileToWriteTo" /> it will overwrite the original file
        /// </summary>
        /// <param name="fileToRead"></param>
        /// <param name="fileToWriteTo"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void DecodeToFile(string fileToRead, string fileToWriteTo = null)
        {
            var fileData = Parse(fileToRead);
            var exportPath = string.IsNullOrEmpty(fileToWriteTo) ? fileToRead : fileToWriteTo;

            WriteToFile(exportPath, fileData);
        }

        private static void WriteToFile(string path, byte[] data)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                throw new DirectoryNotFoundException(
                    $"Cannot write to file '{Path.GetDirectoryName(path)}', directory does not exist");
            }

            File.WriteAllBytes(path, data);
        }

        /// <summary>
        ///     <para>
        ///         Will remove the AES encryption, if file was saved with g_save_format = 0 the result will be the file in Binary
        ///         SII format, otherwise will be in textual format.
        ///     </para>
        ///     if path is not given in <paramref name="fileToWriteTo" /> it will overwrite the original file
        /// </summary>
        /// <param name="fileToRead"></param>
        /// <param name="fileToWriteTo"></param>
        /// <returns></returns>
        public static void RemoveEncryptionFromFile(string fileToRead, string fileToWriteTo = null)
        {
            var fileData = ReadFileData(fileToRead);
            var exportPath = string.IsNullOrEmpty(fileToWriteTo) ? fileToRead : fileToWriteTo;

            WriteToFile(exportPath, RemoveEncryption(fileData));
        }

        private static byte[] RemoveEncryption(byte[] fileData)
        {
            var iv = new byte[0x10];
            var cipherText = new byte[fileData.Length - 0x38];
            Array.Copy(fileData, 0x38, cipherText, 0, cipherText.Length);
            Array.Copy(fileData, 0x24, iv, 0, iv.Length);

            return ZlibStream.UncompressBuffer(AesDecrypt.Decrypt(cipherText, iv));
        }
    }
}
