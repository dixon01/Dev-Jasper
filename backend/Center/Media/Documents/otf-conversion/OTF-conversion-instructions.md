# Conversion of OTF fonts
---
## Font compatibilities
On some Systems running Windows7 or XP there is a compatibility issue for OTF fonts.

**From MSDN:**  
*Windows Forms applications support TrueType fonts and have limited support for OpenType fonts. If you try to use a font that is not supported, such as an unsupported OpenType font or a Bitmap font, an exception will occur.*

The best solution so far to use such a font is to convert it to a TTF font.

## Font conversion using FontForge
### FontForge
Fontfore is a free open source tool available for Windows, Mac and Linux. It can be downloaded form the [project site](http://fontforge.github.io).

### Conversion process
1. Open FontForge
2. Open the OTF font which is to be converted
3. Navigate to File->Create Fonts
4.  In the generate font dialog set:
	- Select TrueType from the font type drop down
	- Choos a new file name (with extension .ttf)
	- Press generate

The new font should be generated. 

### Known Error messages during generation:
- Non-standard Em-size: Click yes.
