using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public class DocumentComparerInternalException : Exception
    {
        public DocumentComparerInternalException(string message) : base(message) { }
    }

    public static class DocumentComparer
    {
        private enum RunElementType
        {
            WordRun,
            MathRun,
            Other
        };

        private enum RunChildElementType
        {
            WordTextElement,
            MathTextElement,
            Other
        };

        private static bool CompareRunCollections(IEnumerable<XElement> runCollection1,
            IEnumerable<XElement> runCollection2)
        {
            var runChildElementCollection1 = runCollection1
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.commentReference &&
                    e.Name != W.rPr &&
                    e.Name != W.proofErr &&
                    e.Name != M.sty &&
                    e.Name != M.rPr)
                .GroupAdjacent(e =>
                {
                    if (e.Name == W.t)
                        return RunChildElementType.WordTextElement;
                    if (e.Name == M.t)
                        return RunChildElementType.MathTextElement;
                    return RunChildElementType.Other;
                });
            var runChildElementCollection2 = runCollection2
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.commentReference &&
                    e.Name != W.rPr &&
                    e.Name != W.proofErr &&
                    e.Name != M.sty &&
                    e.Name != M.rPr)
                .GroupAdjacent(e =>
                {
                    if (e.Name == W.t)
                        return RunChildElementType.WordTextElement;
                    if (e.Name == M.t)
                        return RunChildElementType.MathTextElement;
                    return RunChildElementType.Other;
                });
#if false
        foreach (var item in runChildElementCollection1)
        {
            Console.WriteLine(item.Key);
            foreach (var i2 in item)
            {
                Console.WriteLine(i2.Name.LocalName);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        foreach (var item in runChildElementCollection2)
        {
            Console.WriteLine(item.Key);
            foreach (var i2 in item)
            {
                Console.WriteLine(i2.Name.LocalName);
            }
            Console.WriteLine();
        }
        Environment.Exit(0);
#endif
            bool different = runChildElementCollection1
                .Zip(runChildElementCollection2, (c1, c2) =>
                {
                    if (c1.Key != c2.Key)
                        return false;
                    switch (c1.Key)
                    {
                        case RunChildElementType.WordTextElement:
                        case RunChildElementType.MathTextElement:
                            string string1 = c1.Select(t => (string)t).StringConcatenate();
                            string string2 = c2.Select(t => (string)t).StringConcatenate();
                            if (string1 == string2)
                                return true;
                            else
                                return false;
                        case RunChildElementType.Other:
                            bool d = c1
                                .Zip(c2, (e1, e2) => new { E1 = e1, E2 = e2 })
                                .Select(z => CompareOpenXmlElements(z.E1, z.E2))
                                .Any(b => b == false);
                            if (d == true)
                                return false;
                            else
                                return true;
                    };
                    throw new DocumentComparerInternalException(
                        "Should not have reached this code");
                })
                .Any(b => b == false);
            if (different == true)
                return false;
            else
                return true;
        }

        private static bool CompareOtherCollections(IEnumerable<XElement> collection1,
            IEnumerable<XElement> collection2)
        {
            bool different = collection1
                .Zip(collection2, (e1, e2) => new { E1 = e1, E2 = e2 })
                .Select(z => CompareOpenXmlElements(z.E1, z.E2))
                .Any(b => b == false);
            if (different == true)
                return false;
            else
                return true;
        }

        private static bool CompareElementsWithRuns(XElement element1, XElement element2)
        {
            // todo why is hyperlink and fldChar in the following list???
            var groupedCollection1 = element1
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.rsid &&
                    e.Name != W.proofErr &&
                    e.Name != W.fldChar &&
                    e.Name != W.hyperlink &&
                    e.Name != W.bookmarkStart &&
                    e.Name != W.bookmarkEnd)
                .GroupAdjacent(e =>
                {
                    if (e.Name == W.r)
                        return RunElementType.WordRun;
                    if (e.Name == M.r)
                        return RunElementType.MathRun;
                    return RunElementType.Other;
                });
            var groupedCollection2 = element2
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.rsid &&
                    e.Name != W.proofErr &&
                    e.Name != W.fldChar &&
                    e.Name != W.hyperlink &&
                    e.Name != W.bookmarkStart &&
                    e.Name != W.bookmarkEnd)
                .GroupAdjacent(e =>
                {
                    if (e.Name == W.r)
                        return RunElementType.WordRun;
                    if (e.Name == M.r)
                        return RunElementType.MathRun;
                    return RunElementType.Other;
                });
            if (groupedCollection1.Count() != groupedCollection2.Count())
            {
#if false
            // If this test returns false for a document that you think is erroneous,
            // the following will give clues to the markup that may need to be ignored, or
            // the markup that indicates that the documents are different.
            foreach (var item in groupedCollection1)
            {
                Console.WriteLine(item.Key);
                foreach (var item2 in item)
                {
                    Console.WriteLine(item2.Name.LocalName);
                }
                Console.WriteLine();
            }
            Console.WriteLine("==================");
            foreach (var item in groupedCollection2)
            {
                Console.WriteLine(item.Key);
                foreach (var item2 in item)
                {
                    Console.WriteLine(item2.Name.LocalName);
                }
                Console.WriteLine();
            }
            Environment.Exit(0);
#endif
                return false;
            }
            bool different = groupedCollection1
                .Zip(groupedCollection2, (c1, c2) =>
                {
                    if (c1.Key != c2.Key)
                        return false;
                    switch (c1.Key)
                    {
                        case RunElementType.WordRun:
                        case RunElementType.MathRun:
                            return CompareRunCollections(c1, c2);
                        case RunElementType.Other:
                            return CompareOtherCollections(c1, c2);
                    };
                    throw new DocumentComparerInternalException(
                        "Should not have reached this code");
                })
                .Any(b => b == false);
            if (different == true)
                return false;
            else
                return true;
        }

        // returns true if the elements are the same, otherwise false.
        private static bool CompareOpenXmlElements(XElement element1, XElement element2)
        {
            if (element1.Name != element2.Name)
                return false;
            if (element1
                .Elements()
                .Where(e => e.Name == W.r || e.Name == M.r)
                .Any())
            {
                bool rValue = CompareElementsWithRuns(element1, element2);
                // todo fix
                if (rValue == true)
                    return true;
                else
                    return false;
            }
            if (element1.Nodes().OfType<XText>().Any() &&
                !element1.HasElements)
            {
                if (element1.Value == element2.Value)
                    return true;
                else
                    return false;
            }
            if (element1.IsEmpty && element2.IsEmpty)
                return true;
            var c1 = element1
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.proofErr &&
                    e.Name != W.rsid);
            var c2 = element2
                .Elements()
                .Where(e => e.Name != W.commentRangeStart &&
                    e.Name != W.commentRangeEnd &&
                    e.Name != W.proofErr &&
                    e.Name != W.rsid);
            if (c1.Count() != c2.Count())
                return false;
            var different = c1
                .Zip(c2, (e1, e2) => new { E1 = e1, E2 = e2 })
                .Select(p => CompareOpenXmlElements(p.E1, p.E2))
                .Any(b => b == false);
            if (different == true)
                return false;
            else
                return true;
        }

        // todo
        // we want overload for this that takes WmlDocument, or whatever new class is.
        // also want ability to specify multiple options - a settings object - compare headers, footers, etc.
        //
        // Returns true if docs are the same, otherwise false.
        public static bool CompareDocuments(WordprocessingDocument doc1, WordprocessingDocument doc2)
        {
            XDocument doc1XDoc = doc1.MainDocumentPart.GetXDocument();
            XDocument doc2XDoc = doc2.MainDocumentPart.GetXDocument();
            if (CompareOpenXmlElements(doc1XDoc.Root, doc2XDoc.Root) == false)
                return false;

            // for the current use of this class, only need to compare the main document parts.
#if false
            if (doc1.MainDocumentPart.HeaderParts.Count() != doc2.MainDocumentPart.HeaderParts.Count())
                return false;
            foreach (var pair in doc1
                .MainDocumentPart
                .HeaderParts
                .Zip(doc2.MainDocumentPart.HeaderParts, (d1, d2) =>
                    new
                    {
                        Doc1Header = d1,
                        Doc2Header = d2,
                    }))
            {
                if (CompareOpenXmlElements(pair.Doc1Header.GetXDocument().Root,
                    pair.Doc2Header.GetXDocument().Root) == false)
                    return false;
            }
            if (doc1.MainDocumentPart.FooterParts.Count() != doc2.MainDocumentPart.FooterParts.Count())
                return false;
            foreach (var pair in doc1
                .MainDocumentPart
                .FooterParts
                .Zip(doc2.MainDocumentPart.FooterParts, (d1, d2) =>
                    new
                    {
                        Doc1Footer = d1,
                        Doc2Footer = d2,
                    }))
            {
                if (CompareOpenXmlElements(pair.Doc1Footer.GetXDocument().Root,
                    pair.Doc2Footer.GetXDocument().Root) == false)
                    return false;
            }
            if ((doc1.MainDocumentPart.FootnotesPart == null) != (doc2.MainDocumentPart.FootnotesPart == null))
                return false;
            if (doc1.MainDocumentPart.FootnotesPart != null)
            {
                if (CompareOpenXmlElements(doc1.MainDocumentPart.FootnotesPart.GetXDocument().Root,
                    doc2.MainDocumentPart.FootnotesPart.GetXDocument().Root) == false)
                    return false;
            }
            if ((doc1.MainDocumentPart.EndnotesPart == null) != (doc2.MainDocumentPart.EndnotesPart == null))
                return false;
            if (doc1.MainDocumentPart.EndnotesPart != null)
            {
                if (CompareOpenXmlElements(doc1.MainDocumentPart.EndnotesPart.GetXDocument().Root,
                    doc2.MainDocumentPart.EndnotesPart.GetXDocument().Root) == false)
                    return false;
            }
#endif
            return true;
        }
    }
}
