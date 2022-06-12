using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CLSEncoderDecoder.Import;

internal static class ClsImporter
{
    internal static ClsColorSet Import(Stream stream)
    {
        var data = ReadData(stream);
        return ParseLoadedData(data);
    }

    private static LoadedColorSetData ReadData(Stream bytes)
    {
        using BinaryReader reader = new(bytes);
        LoadedColorSetData data = new();

        data.SlccText = reader.ReadBytes(4);
        data.SomeMagicNumber = reader.ReadUInt16();

        // header
        {
            uint length = reader.ReadUInt32();
            if (length > 131080)
                throw new FormatException($"The file header is unreasonably long ({length} bytes)");
            ushort asciiStringLength = reader.ReadUInt16();
            data.AsciiName = reader.ReadBytes(asciiStringLength);
            data.Useless0InHeader = reader.ReadUInt32();
            ushort utf8StringLength = reader.ReadUInt16();
            data.Utf8Name = reader.ReadBytes(utf8StringLength);
        }

        // body
        data.ProbablyNumberOfChannels = reader.ReadUInt32();
        uint colorCount = reader.ReadUInt32();
        if (colorCount > 4096)
            throw new FormatException($"There are unreasonably many colors ({colorCount} colors)");
        List<byte[]> colors = new();
        uint colorsBlockLength = reader.ReadUInt32();
        for (int i = 0; i < colorCount; i++)
        {
            uint blockLength = reader.ReadUInt32();
            if (blockLength > 1024)
                throw new FormatException($"The color block is unreasonably long ({blockLength} bytes)");
            colors.Add(reader.ReadBytes((int)blockLength));
        }
        data.Colors = colors;

        return data;
    }

    private static ClsColorSet ParseLoadedData(LoadedColorSetData data)
    {
        if (data.AsciiName is null || data.Utf8Name is null || data.SlccText is null || data.Colors is null)
            throw new ArgumentException();
        string slccString = Encoding.ASCII.GetString(data.SlccText);
        if (slccString != "SLCC")
            throw new FormatException($@"The file's format is not ""SLCC"" but ""{slccString}""");
        if (data.SomeMagicNumber != 256)
            throw new FormatException("SomeMagicNumber has to be 1`");
        string asciiName = Encoding.ASCII.GetString(data.AsciiName);
        string utf8Name;
        try
        {
            utf8Name = Encoding.UTF8.GetString(data.Utf8Name);
        }
        catch (Exception e)
        {
            throw new FormatException("Couldn't parse the utf-8 name", e);
        }
        if (data.ProbablyNumberOfChannels > 4)
            throw new FormatException("Invalid value of \"probably number of channels\" field");
        List<ClsColor> parsedColors = new();
        foreach (var bytes in data.Colors)
        {
            if (bytes.Length < 3)
                throw new FormatException($"Couldn't read color RGBA values, there are only {bytes.Length} bytes");
            ClsColor color = new()
            {
                Red = bytes[0],
                Green = bytes[1],
                Blue = bytes[2],
                Alpha = bytes.Length >= 4 ? bytes[3] : (byte)255
            };
            parsedColors.Add(color);
        }

        ClsColorSet colorSet = new(parsedColors, asciiName, utf8Name);
        return colorSet;
    }
}