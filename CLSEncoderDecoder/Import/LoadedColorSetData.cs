using System.Collections.Generic;

namespace CLSEncoderDecoder.Import;

internal class LoadedColorSetData
{
    public byte[]? SlccText { get; set; }
    public ushort SomeMagicNumber { get; set; }
    public byte[]? AsciiName { get; set; }
    public uint Useless0InHeader { get; set; }
    public byte[]? Utf8Name { get; set; }
    public uint ProbablyNumberOfChannels { get; set; }
    public List<byte[]>? Colors { get; set; }
}