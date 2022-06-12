using System.IO;
using System.Linq;
using System.Text;

namespace CLSEncoderDecoder.Export;

internal static class ClsExporter
{
    internal static byte[] SaveToArray(ClsColorSet set)
    {
        using MemoryStream stream = new();
        WriteIntoStream(stream, set);
        return stream.ToArray();
    }

    internal static void Save(ClsColorSet set, string path)
    {
        using var stream = File.Create(path);
        WriteIntoStream(stream, set);
    }

    private static string ConvertToAsciiRepresentation(string str)
    {
        var chars = str.Select(static c => c < 128 ? c : '?').ToArray();
        return new string(chars);
    }

    private static void WriteIntoStream(Stream stream, ClsColorSet set)
    {
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(Encoding.ASCII.GetBytes("SLCC"));
        writer.Write((ushort)256);
        byte[] asciiName = Encoding.ASCII.GetBytes(ConvertToAsciiRepresentation(set.AsciiName));
        byte[] utf8Name = Encoding.UTF8.GetBytes(set.Utf8Name);
        writer.Write((uint)(asciiName.Length + utf8Name.Length + 8));
        writer.Write((ushort)asciiName.Length);
        writer.Write(asciiName);
        writer.Write((uint)0);
        writer.Write((ushort)utf8Name.Length);
        writer.Write(utf8Name);
        writer.Write((uint)4);
        writer.Write((uint)set.Colors.Count);
        writer.Write((uint)(set.Colors.Count * 12));
        foreach (var color in set.Colors)
        {
            writer.Write((uint)8);
            writer.Write(color.Red);
            writer.Write(color.Green);
            writer.Write(color.Blue);
            writer.Write(color.Alpha);
            writer.Write((uint)0);
        }
    }
}