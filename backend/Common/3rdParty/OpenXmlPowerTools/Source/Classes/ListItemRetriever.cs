/***************************************************************************

Copyright (c) Microsoft Corporation 2010.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license
can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public partial class WmlDocument : OpenXmlPowerToolsDocument
    {
        public string RetrieveListItem(XElement paragraph, string bulletReplacementString)
        {
            return ListItemRetriever.RetrieveListItem(this, paragraph, bulletReplacementString);
        }
    }

    public class ListItemRetriever
    {
        private class ListItemInfo
        {
            public bool IsListItem;
            public XElement Lvl;
            public int? Start;
            public int? AbstractNumId;
            public ListItemInfo(bool isListItem)
            {
                IsListItem = isListItem;
            }
        }

        private class LevelNumbers
        {
            public int[] LevelNumbersArray;
        }

        private static ListItemInfo GetListItemInfoByNumIdAndIlvl(XDocument numbering,
            XDocument styles, int numId, int ilvl)
        {
            if (numId == 0)
                return new ListItemInfo(false);
            ListItemInfo listItemInfo = new ListItemInfo(true);
            XElement num = numbering.Root.Elements(W.num)
                .Where(e => (int)e.Attribute(W.numId) == numId).FirstOrDefault();
            if (num == null)
                return new ListItemInfo(false);
            listItemInfo.AbstractNumId = (int?)num.Elements(W.abstractNumId)
                .Attributes(W.val).FirstOrDefault();
            XElement lvlOverride = num.Elements(W.lvlOverride)
                .Where(e => (int)e.Attribute(W.ilvl) == ilvl).FirstOrDefault();
            // If there is a w:lvlOverride element, and if the w:lvlOverride contains a
            // w:lvl element, then return it.  Otherwise, go look in the abstract numbering
            // definition.
            if (lvlOverride != null)
            {
                // Get the startOverride, if there is one.
                listItemInfo.Start = (int?)num.Elements(W.lvlOverride)
                    .Where(o => (int)o.Attribute(W.ilvl) == ilvl).Elements(W.startOverride)
                    .Attributes(W.val).FirstOrDefault();
                listItemInfo.Lvl = lvlOverride.Element(W.lvl);
                if (listItemInfo.Lvl != null)
                {
                    if (listItemInfo.Start == null)
                        listItemInfo.Start = (int?)listItemInfo.Lvl.Elements(W.start)
                            .Attributes(W.val).FirstOrDefault();
                    return listItemInfo;
                }
            }
            int? a = listItemInfo.AbstractNumId;
            XElement abstractNum = numbering.Root.Elements(W.abstractNum)
                .Where(e => (int)e.Attribute(W.abstractNumId) == a).FirstOrDefault();
            string numStyleLink = (string)abstractNum.Elements(W.numStyleLink)
                .Attributes(W.val).FirstOrDefault();
            if (numStyleLink != null)
            {
                XElement style = styles.Root.Elements(W.style)
                    .Where(e => (string)e.Attribute(W.styleId) == numStyleLink)
                    .FirstOrDefault();
                XElement numPr = style.Elements(W.pPr).Elements(W.numPr).FirstOrDefault();
                int lNumId = (int)numPr.Elements(W.numId).Attributes(W.val)
                    .FirstOrDefault();
                return GetListItemInfoByNumIdAndIlvl(numbering, styles, lNumId, ilvl);
            }
            for (int l = ilvl; l >= 0; --l)
            {
                listItemInfo.Lvl = abstractNum.Elements(W.lvl)
                    .Where(e => (int)e.Attribute(W.ilvl) == l).FirstOrDefault();
                if (listItemInfo.Lvl == null)
                    continue;
                if (listItemInfo.Start == null)
                    listItemInfo.Start = (int?)listItemInfo.Lvl.Elements(W.start)
                        .Attributes(W.val).FirstOrDefault();
                return listItemInfo;
            }
            return new ListItemInfo(false);
        }

        private static ListItemInfo GetListItemInfoByNumIdAndStyleId(XDocument numbering,
            XDocument styles, int numId, string paragraphStyle)
        {
            // If you have to find the w:lvl by style id, then we can't find it in the
            // w:lvlOverride, as that requires that you have determined the level already.
            ListItemInfo listItemInfo = new ListItemInfo(true);
            XElement num = numbering.Root.Elements(W.num)
                .Where(e => (int)e.Attribute(W.numId) == numId).FirstOrDefault();
            listItemInfo.AbstractNumId = (int)num.Elements(W.abstractNumId)
                .Attributes(W.val).FirstOrDefault();
            int? a = listItemInfo.AbstractNumId;
            XElement abstractNum = numbering.Root.Elements(W.abstractNum)
                .Where(e => (int)e.Attribute(W.abstractNumId) == a).FirstOrDefault();
            string numStyleLink = (string)abstractNum.Element(W.numStyleLink)
                .Attributes(W.val).FirstOrDefault();
            if (numStyleLink != null)
            {
                XElement style = styles.Root.Elements(W.style)
                    .Where(e => (string)e.Attribute(W.styleId) == numStyleLink)
                    .FirstOrDefault();
                XElement numPr = style.Elements(W.pPr).Elements(W.numPr).FirstOrDefault();
                int lNumId = (int)numPr.Elements(W.numId).Attributes(W.val).FirstOrDefault();
                return GetListItemInfoByNumIdAndStyleId(numbering, styles, lNumId,
                    paragraphStyle);
            }
            listItemInfo.Lvl = abstractNum.Elements(W.lvl)
                .Where(e => (string)e.Element(W.pStyle) == paragraphStyle).FirstOrDefault();
            listItemInfo.Start = (int?)listItemInfo.Lvl.Elements(W.start).Attributes(W.val)
                .FirstOrDefault();
            return listItemInfo;
        }

        private static ListItemInfo GetListItemInfo(XDocument numbering, XDocument styles,
            XElement paragraph)
        {
            // The following is an optimization - only determine ListItemInfo once for a
            // paragraph.
            ListItemInfo listItemInfo = paragraph.Annotation<ListItemInfo>();
            if (listItemInfo != null)
                return listItemInfo;
            XElement paragraphNumberingProperties = paragraph.Elements(W.pPr)
                .Elements(W.numPr).FirstOrDefault();
            string paragraphStyle = (string)paragraph.Elements(W.pPr).Elements(W.pStyle)
                .Attributes(W.val).FirstOrDefault();
            if (paragraphNumberingProperties != null &&
                paragraphNumberingProperties.Element(W.numId) != null)
            {
                // Paragraph numbering properties must contain a numId.
                int numId = (int)paragraphNumberingProperties.Elements(W.numId)
                    .Attributes(W.val).FirstOrDefault();
                int? ilvl = (int?)paragraphNumberingProperties.Elements(W.ilvl)
                    .Attributes(W.val).FirstOrDefault();
                if (ilvl != null)
                {
                    listItemInfo = GetListItemInfoByNumIdAndIlvl(numbering, styles, numId,
                        (int)ilvl);
                    paragraph.AddAnnotation(listItemInfo);
                    return listItemInfo;
                }
                if (paragraphStyle != null)
                {
                    listItemInfo = GetListItemInfoByNumIdAndStyleId(numbering, styles,
                        numId, paragraphStyle);
                    paragraph.AddAnnotation(listItemInfo);
                    return listItemInfo;
                }
                listItemInfo = new ListItemInfo(false);
                paragraph.AddAnnotation(listItemInfo);
                return listItemInfo;
            }
            if (paragraphStyle != null)
            {
                XElement style = styles.Root.Elements(W.style).Where(s =>
                    (string)s.Attribute(W.type) == "paragraph" &&
                    (string)s.Attribute(W.styleId) == paragraphStyle).FirstOrDefault();
                if (style != null)
                {
                    XElement styleNumberingProperties = style.Elements(W.pPr)
                        .Elements(W.numPr).FirstOrDefault();
                    if (styleNumberingProperties != null &&
                        styleNumberingProperties.Element(W.numId) != null)
                    {
                        int numId = (int)styleNumberingProperties.Elements(W.numId)
                            .Attributes(W.val).FirstOrDefault();
                        int? ilvl = (int?)styleNumberingProperties.Elements(W.ilvl)
                            .Attributes(W.val).FirstOrDefault();
                        if (ilvl == null)
                            ilvl = 0;
                        listItemInfo = GetListItemInfoByNumIdAndIlvl(numbering, styles,
                            numId, (int)ilvl);
                        paragraph.AddAnnotation(listItemInfo);
                        return listItemInfo;
                    }
                }
            }
            listItemInfo = new ListItemInfo(false);
            paragraph.AddAnnotation(listItemInfo);
            return listItemInfo;
        }

        private static IEnumerable<LevelNumbers> ParagraphsToConsiderWhenCounting(
            XDocument numbering, XDocument styles, XElement paragraph, int levelNumber)
        {
            ListItemInfo listItemInfo = GetListItemInfo(numbering, styles, paragraph);
            int? lvlRestart = (int?)listItemInfo.Lvl.Elements(W.lvlRestart)
                .Attributes(W.val).FirstOrDefault();
            int paragraphLevel = (int)listItemInfo.Lvl.Attribute(W.ilvl);
            IEnumerable<XElement> paragraphsBeforeSelf = paragraph
                .ElementsBeforeSelfReverseDocumentOrder().Where(e => e.Name == W.p);
            foreach (var p in paragraphsBeforeSelf)
            {
                ListItemInfo pListItemInfo = GetListItemInfo(numbering, styles, p);
                if (!pListItemInfo.IsListItem ||
                    pListItemInfo.AbstractNumId != listItemInfo.AbstractNumId)
                    continue;
                LevelNumbers pLevelNumbers = p.Annotation<LevelNumbers>();
                int pLevel = (int)pListItemInfo.Lvl.Attribute(W.ilvl);
                if (pLevel > levelNumber)
                    yield return pLevelNumbers;
                if (lvlRestart == null)
                {
                    if (pLevel < levelNumber)
                        yield break;
                }
                else
                {
                    if (pLevel < levelNumber && pLevel > lvlRestart - 1)
                        continue;
                    else if (pLevel < levelNumber)
                        yield break;
                }
                yield return pLevelNumbers;
            }
        }

        private static int GetLevelNumberForLevel(XDocument numbering, XDocument styles,
            XElement paragraph, int level)
        {
            ListItemInfo listItemInfo = GetListItemInfo(numbering, styles, paragraph);
            int paragraphLevel = (int)listItemInfo.Lvl.Attribute(W.ilvl);
            var paragraphsToConsider = ParagraphsToConsiderWhenCounting(numbering, styles,
                paragraph, level)
                .Select(o => o.LevelNumbersArray.Take(paragraphLevel + 1)
                    .Select(z => z.ToString() + ".").StringConcatenate())
                .GroupBy(o => o);
            int levelNumberForLevel = paragraphsToConsider.Count();
            return levelNumberForLevel;
        }

        private static int[] GetLevelNumbers(XDocument numbering, XDocument styles,
            XElement paragraph)
        {
            IEnumerable<XElement> paragraphsBeforeSelf = paragraph
                .ElementsBeforeSelfReverseDocumentOrder().Where(e => e.Name == W.p);
            int level;
            ListItemInfo listItemInfo = GetListItemInfo(numbering, styles, paragraph);
            level = (int)listItemInfo.Lvl.Attribute(W.ilvl);
            List<int> levelNumbers = new List<int>();
            for (int indentationLevel = 0; indentationLevel <= level; ++indentationLevel)
            {
                XElement currentIndentLvl = GetRelatedLevel(listItemInfo.Lvl,
                    indentationLevel);
                int? start = (int?)currentIndentLvl.Elements(W.start).Attributes(W.val)
                    .FirstOrDefault();
                if (start == null)
                    start = 1;
                XElement paragraphWithSameAbstractNumId = paragraphsBeforeSelf
                    .FirstOrDefault(p =>
                    {
                        ListItemInfo pListItemInfo = GetListItemInfo(numbering, styles, p);
                        return pListItemInfo.IsListItem &&
                            pListItemInfo.AbstractNumId == listItemInfo.AbstractNumId;
                    });
                if (paragraphWithSameAbstractNumId != null)
                {
                    LevelNumbers pLevelNumbers = paragraphWithSameAbstractNumId
                        .Annotation<LevelNumbers>();
                    if (pLevelNumbers.LevelNumbersArray.Length > indentationLevel)
                    {
                        if (indentationLevel == level)
                            levelNumbers.Add(
                                pLevelNumbers.LevelNumbersArray[indentationLevel] + 1);
                        else
                            levelNumbers.Add(pLevelNumbers
                                .LevelNumbersArray[indentationLevel]);
                        continue;
                    }
                }

                if (level == indentationLevel)
                {
                    int c1 = GetLevelNumberForLevel(numbering, styles, paragraph,
                        indentationLevel);
                    int? start2 = listItemInfo.Start;
                    if (start2 == null)
                        start2 = 1;
                    levelNumbers.Add(c1 + (int)start2);
                    continue;
                }
                levelNumbers.Add((int)start);
            }
            return levelNumbers.ToArray();
        }

        private static IEnumerable<string> GetFormatTokens(string lvlText)
        {
            int i = 0;
            while (true)
            {
                if (i >= lvlText.Length)
                    yield break;
                if (lvlText[i] == '%' && i <= lvlText.Length - 2)
                {
                    yield return lvlText.Substring(i, 2);
                    i += 2;
                    continue;
                }
                int percentIndex = lvlText.IndexOf('%', i);
                if (percentIndex == -1 || percentIndex > lvlText.Length - 2)
                {
                    yield return lvlText.Substring(i);
                    yield break;
                }
                yield return lvlText.Substring(i, percentIndex - i);
                yield return lvlText.Substring(percentIndex, 2);
                i = percentIndex + 2;
            }
        }

        private static string[] RomanOnes =
        {
            "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"
        };

        private static string[] RomanTens =
        {
            "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"
        };

        private static string[] RomanHundreds =
        {
            "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM", "M"
        };

        private static string[] RomanThousands =
        {
            "", "M", "MM", "MMM", "MMMM", "MMMMM", "MMMMMM", "MMMMMMM", "MMMMMMMM",
            "MMMMMMMMM", "MMMMMMMMMM"
        };

        private static string[] OneThroughNineteen = {
            "one", "two", "three", "four", "five", "six", "seven", "eight",
            "nine", "ten", "eleven", "twelve", "thirteen", "fourteen",
            "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
        };

        private static string[] Tens = {
            "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy",
            "eighty", "ninety"
        };

        private static string[] OrdinalOneThroughNineteen = {
            "first", "second", "third", "fourth", "fifth", "sixth",
            "seventh", "eighth", "ninth", "tenth", "eleventh", "twelfth",
            "thirteenth", "fourteenth", "fifteenth", "sixteenth",
            "seventeenth", "eighteenth", "nineteenth"
        };

        private static string[] OrdinalTenths = {
            "tenth", "twentieth", "thirtieth", "fortieth", "fiftieth",
            "sixtieth", "seventieth", "eightieth", "ninetieth"
        };

        private static string GetLevelText(int levelNumber, string numFmt)
        {
            if (numFmt == "decimal")
            {
                return levelNumber.ToString();
            }
            if (numFmt == "decimalZero")
            {
                if (levelNumber <= 9)
                    return "0" + levelNumber.ToString();
                else
                    return levelNumber.ToString();
            }
            if (numFmt == "upperRoman")
            {
                int ones = levelNumber % 10;
                int tens = (levelNumber % 100) / 10;
                int hundreds = (levelNumber % 1000) / 100;
                int thousands = levelNumber / 1000;
                return RomanThousands[thousands] + RomanHundreds[hundreds] +
                    RomanTens[tens] + RomanOnes[ones];
            }
            if (numFmt == "lowerRoman")
            {
                int ones = levelNumber % 10;
                int tens = (levelNumber % 100) / 10;
                int hundreds = (levelNumber % 1000) / 100;
                int thousands = levelNumber / 1000;
                return (RomanThousands[thousands] + RomanHundreds[hundreds] +
                    RomanTens[tens] + RomanOnes[ones]).ToLower();
            }
            if (numFmt == "upperLetter")
            {
                string a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                int c = (levelNumber - 1) / 26;
                int n = (levelNumber - 1) % 26;
                char x = a[n];
                return "".PadRight(c + 1, x);
            }
            if (numFmt == "lowerLetter")
            {
                string a = "abcdefghijklmnopqrstuvwxyz";
                int c = (levelNumber - 1) / 26;
                int n = (levelNumber - 1) % 26;
                char x = a[n];
                return "".PadRight(c + 1, x);
            }
            if (numFmt == "ordinal")
            {
                string suffix;
                if (levelNumber % 100 == 11 || levelNumber % 100 == 12 ||
                    levelNumber % 100 == 13)
                    suffix = "th";
                else if (levelNumber % 10 == 1)
                    suffix = "st";
                else if (levelNumber % 10 == 2)
                    suffix = "nd";
                else if (levelNumber % 10 == 3)
                    suffix = "rd";
                else
                    suffix = "th";
                return levelNumber.ToString() + suffix;
            }
            if (numFmt == "cardinalText")
            {
                string result = "";
                int t1 = levelNumber / 1000;
                int t2 = levelNumber % 1000;
                if (t1 >= 1)
                    result += OneThroughNineteen[t1 - 1] + " thousand";
                if (t1 >= 1 && t2 == 0)
                    return result.Substring(0, 1).ToUpper() +
                        result.Substring(1);
                if (t1 >= 1)
                    result += " ";
                int h1 = (levelNumber % 1000) / 100;
                int h2 = levelNumber % 100;
                if (h1 >= 1)
                    result += OneThroughNineteen[h1 - 1] + " hundred";
                if (h1 >= 1 && h2 == 0)
                    return result.Substring(0, 1).ToUpper() +
                        result.Substring(1);
                if (h1 >= 1)
                    result += " ";
                int z = levelNumber % 100;
                if (z <= 19)
                    result += OneThroughNineteen[z - 1];
                else
                {
                    int x = z / 10;
                    int r = z % 10;
                    result += Tens[x - 1];
                    if (r >= 1)
                        result += "-" + OneThroughNineteen[r - 1];
                }
                return result.Substring(0, 1).ToUpper() +
                    result.Substring(1);
            }
            if (numFmt == "ordinalText")
            {
                string result = "";
                int t1 = levelNumber / 1000;
                int t2 = levelNumber % 1000;
                if (t1 >= 1 && t2 != 0)
                    result += OneThroughNineteen[t1 - 1] + " thousand";
                if (t1 >= 1 && t2 == 0)
                {
                    result += OneThroughNineteen[t1 - 1] + " thousandth";
                    return result.Substring(0, 1).ToUpper() +
                        result.Substring(1);
                }
                if (t1 >= 1)
                    result += " ";
                int h1 = (levelNumber % 1000) / 100;
                int h2 = levelNumber % 100;
                if (h1 >= 1 && h2 != 0)
                    result += OneThroughNineteen[h1 - 1] + " hundred";
                if (h1 >= 1 && h2 == 0)
                {
                    result += OneThroughNineteen[h1 - 1] + " hundredth";
                    return result.Substring(0, 1).ToUpper() +
                        result.Substring(1);
                }
                if (h1 >= 1)
                    result += " ";
                int z = levelNumber % 100;
                if (z <= 19)
                    result += OrdinalOneThroughNineteen[z - 1];
                else
                {
                    int x = z / 10;
                    int r = z % 10;
                    if (r == 0)
                        result += OrdinalTenths[x - 1];
                    else
                        result += Tens[x - 1];
                    if (r >= 1)
                        result += "-" + OrdinalOneThroughNineteen[r - 1];
                }
                return result.Substring(0, 1).ToUpper() +
                    result.Substring(1);
            }
            if (numFmt == "bullet")
                return "";
            // This method needs to be enhanced to support all languages and
            // all number formats.
            return levelNumber.ToString();
        }

        private static XElement GetRelatedLevel(XElement lvl, int level)
        {
            XElement parent = lvl.Parent;
            XElement newLvl;
            if (parent.Name == W.lvlOverride)
            {
                newLvl = parent.Parent.Elements(W.lvlOverride).Elements(W.lvl)
                    .Where(e => (int)e.Attribute(W.ilvl) == level).FirstOrDefault();
                if (newLvl != null)
                    return newLvl;
                int abstractNumId = (int)parent.Parent.Elements(W.abstractNumId)
                    .Attributes(W.val).FirstOrDefault();
                XElement abstractNum = lvl.Ancestors().Last().Elements(W.abstractNum)
                    .Where(e => (int)e.Attribute(W.abstractNumId) == abstractNumId)
                    .FirstOrDefault();
                newLvl = abstractNum.Elements(W.lvl)
                    .Where(e => (int)e.Attribute(W.ilvl) == level).FirstOrDefault();
                return newLvl;
            }
            newLvl = parent.Elements(W.lvl).Where(e => (int)e.Attribute(W.ilvl) == level)
                .FirstOrDefault();
            return newLvl;
        }

        private static string FormatListItem(XElement lvl, int[] levelNumbers,
            string lvlText, string bulletReplacementString)
        {
            if (bulletReplacementString != null)
            {
                StringBuilder sb = new StringBuilder(lvlText);
                sb.Replace("\xF076", bulletReplacementString);
                sb.Replace("\xF0D8", bulletReplacementString);
                sb.Replace("\xF0A7", bulletReplacementString);
                sb.Replace("\xF0B7", bulletReplacementString);
                sb.Replace("\xF0A8", bulletReplacementString);
                sb.Replace("\xF0FC", bulletReplacementString);
                lvlText = sb.ToString();
            }
            string[] formatTokens = GetFormatTokens(lvlText).ToArray();
            bool isLgl = lvl.Elements(W.isLgl).Any();
            string listItem = formatTokens.Select((t, l) =>
            {
                if (t.Substring(0, 1) != "%")
                    return t;
                int indentationLevel = Int32.Parse(t.Substring(1)) - 1;
                int levelNumber = levelNumbers[indentationLevel];
                string levelText;
                XElement rlvl = GetRelatedLevel(lvl, indentationLevel);
                string numFmtForLevel = (string)rlvl.Elements(W.numFmt).Attributes(W.val)
                    .FirstOrDefault();
                if (isLgl && numFmtForLevel != "decimalZero")
                    numFmtForLevel = "decimal";
                levelText = GetLevelText(levelNumber, numFmtForLevel);
                return levelText;
            }).StringConcatenate();
            return listItem + " ";
        }

        public static string RetrieveListItem(WmlDocument document,
            XElement paragraph, string bulletReplacementString)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(document))
            {
                using (WordprocessingDocument wdoc = streamDoc.GetWordprocessingDocument())
                {
                    return RetrieveListItem(wdoc, paragraph, bulletReplacementString);
                }
            }
        }

        public static string RetrieveListItem(WordprocessingDocument wordDoc,
            XElement paragraph, string bulletReplacementString)
        {
            string pt = paragraph.Elements(W.r).Elements(W.t).Select(e => e.Value)
                .StringConcatenate();
            NumberingDefinitionsPart numberingDefinitionsPart =
                wordDoc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingDefinitionsPart == null)
                return null;
            StyleDefinitionsPart styleDefinitionsPart = wordDoc.MainDocumentPart
                .StyleDefinitionsPart;
            if (styleDefinitionsPart == null)
                return null;
            XDocument numbering = numberingDefinitionsPart.GetXDocument();
            XDocument styles = styleDefinitionsPart.GetXDocument();
            ListItemInfo listItemInfo = GetListItemInfo(numbering, styles, paragraph);
            if (listItemInfo.IsListItem)
            {
                string lvlText = (string)listItemInfo.Lvl.Elements(W.lvlText)
                    .Attributes(W.val).FirstOrDefault();
                int[] levelNumbers = GetLevelNumbers(numbering, styles, paragraph);
                paragraph.AddAnnotation(new LevelNumbers()
                {
                    LevelNumbersArray = levelNumbers
                });
                string listItem = FormatListItem(listItemInfo.Lvl, levelNumbers, lvlText,
                    bulletReplacementString);
                return listItem;
            }
            return null;
        }
    }
}
