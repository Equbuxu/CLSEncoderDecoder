using CLSEncoderDecoder;

namespace CLSEncoderDecoderTest;

public class ClsColorSetTests
{
    [Fact]
    public void Load_ByteArray_DoesNotThrow()
    {
        var bytes = File.ReadAllBytes(@"files\valid-palette.cls");
        ClsColorSet.Load(bytes);
    }

    [Fact]
    public void Load_File_DoesNotThrow()
    {
        ClsColorSet.Load(@"files\valid-palette.cls");
    }

    [Fact]
    public void Load_FileWith1Color_ColorIsCorrect()
    {
        var set = ClsColorSet.Load(@"files\AABBCC.cls");
        Assert.Collection(
            set.Colors,
            a => Assert.Equal(new ClsColor(0xAA, 0xBB, 0xCC, 0xFF), a));
    }

    [Fact]
    public void Load_Utf8Name_ParsedCorrectly()
    {
        string expectedUtf8 = @"a¡ࠀ𒀀";
        string expectedAscii = "a????";
        var colorSet = ClsColorSet.Load(@"files\unicode.cls");
        Assert.Equal(expectedAscii, colorSet.AsciiName);
        Assert.Equal(expectedUtf8, colorSet.Utf8Name);
    }

    [Theory]
    [InlineData(@"files\valid-palette.cls")]
    [InlineData(@"files\unicode.cls")]
    [InlineData(@"files\AABBCC.cls")]
    public void Save_LoadedFileIntoBytes_GetSameFile(string path)
    {
        var origFile = ClsColorSet.Load(path);
        var saved = origFile.SaveToArray();
        var parsed = ClsColorSet.Load(saved);
        Assert.Equal(origFile, parsed);
    }
}