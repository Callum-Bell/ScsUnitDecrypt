using System;
using CommandLine;
using ScsUnitDecrypt.Core;
using ScsUnitDecrypt.Core.Common;

namespace ScsUnitDecrypt
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    Console.WriteLine($"Decrypting file: '{o.InputFilePath}'...");

                    try
                    {
                        if (o.OnlyRemoveEncryption)
                        {
                            UnitDecoder.RemoveEncryptionFromFile(o.InputFilePath, o.OutputFilePath);
                        }
                        else
                        {
                            var options = DecodeOptions.None;
                            if (o.AddFloatComments) options |= DecodeOptions.FloatComments;
                            var decoder = new UnitDecoder(options);
                            decoder.DecodeToFile(o.InputFilePath, o.OutputFilePath);
                        }

                        Console.WriteLine("Done");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex}");
                    }
                });
        }

        private class Options
        {
            [Value(0, HelpText = "Location of the unit file to decrypt")]
            public string InputFilePath { get; set; }

            [Option('o', "output", Required = false,
                HelpText =
                    "Will write the decrypted content to the specified location and not overwrite the original.")]
            public string OutputFilePath { get; set; }

            [Option("only-remove-encryption", Required = false, Default = false,
                HelpText =
                    "Will (only) remove the encryption, if file is saved with format 0 it will result in the save in binary format.")]
            public bool OnlyRemoveEncryption { get; set; }

            [Option("float-comments", Required = false, Default = false,
                HelpText =
                    "[g_save_format 0 only] Will add comments to float values so it is more easy to read the value when it is originally in hexadecimal form")]
            public bool AddFloatComments { get; set; }
        }
    }
}