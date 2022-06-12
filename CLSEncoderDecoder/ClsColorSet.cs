using System;
using System.Collections.Generic;
using System.IO;
using CLSEncoderDecoder.Export;
using CLSEncoderDecoder.Import;

namespace CLSEncoderDecoder;

public class ClsColorSet
{
    private string asciiName;
    private string utf8Name;

    protected bool Equals(ClsColorSet other)
    {
        if (!(AsciiName == other.AsciiName && Utf8Name == other.Utf8Name && Colors.Count == other.Colors.Count))
            return false;
        for (int i = 0; i < Colors.Count; i++)
        {
            if (Colors[i] != other.Colors[i])
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ClsColorSet)obj);
    }

    public ClsColorSet(List<ClsColor> colors, string asciiName, string utf8Name)
    {
        Colors = colors;
        this.asciiName = asciiName;
        this.utf8Name = utf8Name;
    }

    public ClsColorSet(List<ClsColor> colors, string name)
    {
        Colors = colors;
        this.asciiName = "";
        this.utf8Name = "";
        SetBothNames(name);
    }

    public string AsciiName
    {
        get => asciiName;
        set
        {
            if (value.Length > 64)
                throw new InvalidOperationException("Max name length is 64");
            asciiName = value;
        }
    }

    public string Utf8Name
    {
        get => utf8Name;
        set
        {
            if (value.Length > 64)
                throw new InvalidOperationException("Max name length is 64");
            utf8Name = value;
        }
    }

    public void SetBothNames(string name)
    {
        AsciiName = name;
        Utf8Name = name;
    }

    public List<ClsColor> Colors { get; set; }

    public static ClsColorSet Load(string path)
    {
        using var stream = File.OpenRead(path);
        return ClsImporter.Import(stream);
    }

    public static ClsColorSet Load(byte[] fileBytes)
    {
        using var stream = new MemoryStream(fileBytes);
        return ClsImporter.Import(stream);
    }

    public byte[] SaveToArray() => ClsExporter.SaveToArray(this);

    public void Save(string path) => ClsExporter.Save(this, path);
}