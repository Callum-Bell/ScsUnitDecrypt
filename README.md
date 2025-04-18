<h1 align="center">ScsUnitDecrypt</h1>
<p align="center"><b>Decrypt ETS2 / ATS Save Files with g_save_format 0</b></p>

---

## 📦 Overview

**ScsUnitDecrypt** is a tool for decrypting save files from *Euro Truck Simulator 2* and *American Truck Simulator* that were saved using:

```
g_save_format 0
```

This setting stores save data in a **binary SII format**, which this tool can convert back into a readable text format.

---

## 🖥 Console Usage

```bash
ScsUnitDecrypt [input_file_path] [options]
```

### 🔧 Available Options

| Option                     | Description |
|---------------------------|-------------|
| `-o, --output <path>`     | Writes the decrypted content to the specified file path instead of overwriting the original. |
| `--only-remove-encryption`| Only removes encryption. If the save file is already in format 0, this results in a still-encrypted binary file without further decoding. |
| `--float-comments`        | *(g_save_format 0 only)* Adds comments next to float values for better readability when values were originally in hexadecimal. |
| `--help`                  | Displays the help screen. |
| `--version`               | Displays version information. |

---

## 📚 Library Usage

If you're using **ScsUnitDecrypt** as a library in your own C# project:

### 🔨 Basic Usage

```csharp
var decoder = new UnitDecoder();
decoder.DecodeToFile(inputFilePath); // Overwrites original
```

### 📝 Output to New File

```csharp
decoder.DecodeToFile(inputFilePath, outputFilePath); // Keeps original untouched
```

### ⚙️ Options

You can pass `DecodeOptions` into the constructor to customise behavior:

```csharp
var options = new DecodeOptions
{
    WriteFloatComments = true // Add comments next to float values
};

var decoder = new UnitDecoder(options);
decoder.DecodeToFile(inputFilePath);
```

---

## 📦 Dependencies

- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser)
- [Fody](https://www.nuget.org/packages/Fody)
- [Costura.Fody](https://www.nuget.org/packages/Costura.Fody)

---

## 🙌 Special Thanks

- [František Milt](https://github.com/TheLazyTomcat)
  – Creator of [SII_Decrypt](https://github.com/TheLazyTomcat/SII_Decrypt), which served as a reference for this tool.
