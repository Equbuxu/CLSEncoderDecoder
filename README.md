[![Download](https://img.shields.io/badge/nuget-download-blue)](https://www.nuget.org/packages/CLSEncoderDecoder/)

# A simple Encoder/Decoder for Color Set files from Clip Studio Paint (.cls)
## Examples
### Saving
```csharp
ClsColorSet set = new(
    new List<ClsColor>()
    {
        new ClsColor(123, 234, 243, 255),
        new ClsColor(123, 234, 243, 0),
    },
    "Color Set Name");
set.Save("TwoColors.cls");
```
### Loading
```csharp
ClsColorSet set = ClsColorSet.Load(@"LightGrayish.cls");
```
## .cls file format

I reverse-engineered the format myself, so this is not a complete spec. 
I don't know if the values are signed or unsigned, 
but it shouldn't matter for any reasonable color set. I assume unsigned in the code.

CSP seems to treat any colors with non-zero alpha as fully opaque, 
so the only alpha values you should use are 0 and 255.

### Types
|  type   | description                                                                           |
|:-------:|:--------------------------------------------------------------------------------------|
|  int32  | number, 32 bit                                                                        |
|  int16  | number, 16 bit                                                                        |
|  rgba   | 4 bytes with RGBA values                                                              |
|  ascii  | text encoded as ASCII, 1 byte/character (without string terminators or anything else) |
|  utf8   | text encoded as UTF-8, 1-4 bytes/character                                            |

### File sections

#### Signature

| type  | value |
|:-----:|:-----:|
| ascii | SLCC  |
| int16 |  256  |

I put 256 as a part of the signature, but it could also be an indication of the version

#### Header

| type  |                   value                   |
|:-----:|:-----------------------------------------:|
| int32 | length of the rest of the header in bytes |
| int16 |  length of the following string in bytes  |
| ascii |           name of the color set           |
| int32 |                     0                     |
| int16 |  length of the following string in bytes  |
| utf8  |           name of the color set           |

CSP ignored the ASCII name.

The zero in the middle seems to be useless, changing it to any other value doesn't prevent CSP from loading the file.

#### Colors

| type  |                   value                   |
|:-----:|:-----------------------------------------:|
| int32 |                     4                     |
| int32 |             number of colors              |
| int32 |    length of the next section in bytes    |

"4" might be the number of channels, values above 4 prevent CSP from loading the file. 0-3 are treated the same as 4.

The next section contains all colors written one after another in the following format:

| type  |                        value                        |
|:-----:|:---------------------------------------------------:|
| int32 | length of the rest of this block in bytes, always 8 |
| rgba  |                      the color                      |
| int32 |                          0                          |

The 0 in the end doesn't do anything as far as I can tell, but it has to be a zero. 
Curiously, you can omit the 0 and set the length to 4, and the file loads just fine.
Increasing the length and appending more zeros also doesn't break the file.
I don't recommend doing any of that though, as CSP itself outputs colors with length 8 and a trailing 0.