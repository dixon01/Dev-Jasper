using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace OpenXmlPowerTools
{
    public partial class WmlDocument : OpenXmlPowerToolsDocument
    {
        public WmlDocument MergeComments(WmlDocument other)
        {
            return CommentMerger.MergeComments(this, other);
        }
    }

    public class CommentMergerInternalException : Exception
    {
        public CommentMergerInternalException(string message) : base(message) { }
    }

    public class CommentMergerDifferingContentsException : Exception
    {
        public CommentMergerDifferingContentsException(string message) : base(message) { }
    }

    public class CommentMergerUnlockedDocumentException : Exception
    {
        public CommentMergerUnlockedDocumentException(string message) : base(message) { }
    }

    public class CommentMerger
    {
        public static WmlDocument MergeComments(WmlDocument document1, WmlDocument document2)
        {
            return MergeComments(document1, document2, true);
        }

        public static WmlDocument MergeComments(WmlDocument document1, WmlDocument document2,
            bool ensureLocked)
        {
            WmlDocument cDoc1 = new WmlDocument(document1);
            WmlDocument cDoc2 = new WmlDocument(document2);
            using (OpenXmlMemoryStreamDocument streamDoc1 = new OpenXmlMemoryStreamDocument(cDoc1))
            using (WordprocessingDocument doc1 = streamDoc1.GetWordprocessingDocument())
            using (OpenXmlMemoryStreamDocument streamDoc2 = new OpenXmlMemoryStreamDocument(cDoc2))
            using (WordprocessingDocument doc2 = streamDoc2.GetWordprocessingDocument())
            {
                SimplifyMarkupSettings mss = new SimplifyMarkupSettings()
                {
                    RemoveProof = true,
                    RemoveRsidInfo = true,
                    RemoveGoBackBookmark = true,
                };
                MarkupSimplifier.SimplifyMarkup(doc1, mss);
                MarkupSimplifier.SimplifyMarkup(doc2, mss);

                // If documents don't contain the same content, then don't attempt to merge comments.
                bool same = DocumentComparer.CompareDocuments(doc1, doc2);
                if (!same)
                    throw new CommentMergerDifferingContentsException(
                        "Documents do not contain the same content");

                if (doc1.MainDocumentPart.WordprocessingCommentsPart == null &&
                    doc2.MainDocumentPart.WordprocessingCommentsPart == null)
                    return new WmlDocument(document1);
                if (doc1.MainDocumentPart.WordprocessingCommentsPart != null &&
                    doc2.MainDocumentPart.WordprocessingCommentsPart == null)
                    return new WmlDocument(document1);
                if (doc1.MainDocumentPart.WordprocessingCommentsPart == null &&
                    doc2.MainDocumentPart.WordprocessingCommentsPart != null)
                    return new WmlDocument(document2);
                // If either of the documents have no comments, then return the other one.
                if (! doc1.MainDocumentPart.WordprocessingCommentsPart.GetXDocument().Root
                    .Elements(W.comment).Any())
                    return new WmlDocument(document2);
                if (! doc2.MainDocumentPart.WordprocessingCommentsPart.GetXDocument().Root
                    .Elements(W.comment).Any())
                    return new WmlDocument(document1);

                if (ensureLocked)
                {
                    // If either document is not locked (allowing only commenting), don't attempt to
                    // merge comments.
                    if (doc1.ExtendedFilePropertiesPart.GetXDocument().Root
                        .Element(EP.DocSecurity).Value != "8")
                        throw new CommentMergerUnlockedDocumentException(
                            "Document1 is not locked");
                    if (doc2.ExtendedFilePropertiesPart.GetXDocument().Root
                        .Element(EP.DocSecurity).Value != "8")
                        throw new CommentMergerUnlockedDocumentException(
                            "Document2 is not locked");
                }
                
                RenumberCommentsInDoc2(doc1, doc2);

                WmlDocument destDoc = new WmlDocument(document1);

                using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(destDoc))
                {
                    using (WordprocessingDocument destWDoc = streamDoc.GetWordprocessingDocument())
                    {
                        // Merge the comments part.
                        XDocument commentsPartXDoc = new XDocument(
                            new XElement(W.comments,
                                new XAttribute(XNamespace.Xmlns + "w", W.w),
                                doc1.MainDocumentPart.WordprocessingCommentsPart.GetXDocument().Root.Elements(),
                                doc2.MainDocumentPart.WordprocessingCommentsPart.GetXDocument().Root.Elements()));
                        destWDoc.MainDocumentPart.WordprocessingCommentsPart.PutXDocument(commentsPartXDoc);

                        MergeCommentsInPart(doc1.MainDocumentPart, doc2.MainDocumentPart, 
                            destWDoc.MainDocumentPart, commentsPartXDoc);
                    }
                    return streamDoc.GetModifiedWmlDocument();
                }
            }
        }

        private static bool IsCommentElement(XElement e)
        {
            return e.Name == W.commentRangeStart || e.Name == W.commentRangeEnd || 
                e.Name == W.commentReference;
        }

        private static IEnumerable<object> SplitElements(IEnumerable<XElement> elements)
        {
            IEnumerable<object> splitElements = elements
                .Select(e =>
                    {
                        if ((e.Name == W.r && e.Elements(W.t).Any()) ||
                            (e.Name == M.r && e.Elements(M.t).Any()))
                        {
                            var grouped = e.Elements().Where(ce => ce.Name != W.rPr && ce.Name != M.rPr)
                                .GroupAdjacent(ce => 
                                (ce.Name == W.t || ce.Name == M.t));
                            return grouped.Select(g =>
                                {
                                    if (g.Key == true)
                                    {
                                        string s = g.Select(t => (string)t).StringConcatenate();
                                        return s.Select(c => new XElement(e.Name,
                                            e.Attributes(),
                                            e.Elements(W.rPr),
                                            e.Elements(M.rPr),
                                            new XElement(g.First().Name, c)));
                                    }
                                    return g.Select(ce => new XElement(e.Name,
                                        e.Attributes(),
                                        e.Elements(W.rPr),
                                        e.Elements(M.rPr),
                                        ce));
                                });
                        }
                        return (object)e;
                    });
            return splitElements;
        }

        private class RunOrderingInfo
        {
            public int Integer;
            public bool IsRun;

            public RunOrderingInfo(int integer, bool isRun)
            {
                Integer = integer;
                IsRun = isRun;
            }
            public RunOrderingInfo(int integer)
            {
                Integer = integer;
                IsRun = false;
            }
        }

        private static XName[] RunContentElements = new[]
        {
            M.t,
            W.br,
            W.cr,
            W.dayLong,
            W.dayShort,
            W.drawing,
            W.endnoteReference,
            W.fldChar,
            W.footnoteReference,
            W.instrText,
            W.lastRenderedPageBreak,
            W.monthLong,
            W.monthShort,
            W.noBreakHyphen,
            W._object,
            W.ptab,
            W.ruby,
            W.softHyphen,
            W.sym,
            W.t,
            W.tab,
            W.yearLong,
            W.yearShort,
        };

        private static void AddAnnotationsToChildren(XElement parent)
        {
            int index = 1000;
            foreach (var ce in parent
                .Elements().Where(e => e.Name == W.r || e.Name == M.r)
                .Where(r =>
                    {
                        foreach (var item in RunContentElements)
                            if (r.Element(item) != null)
                                return true;
                        return false;
                    }))
            {
                ce.AddAnnotation(new RunOrderingInfo(index, true));
                index += 1000;
            }
            foreach (var ce in parent.Elements().Where(e => e.Annotation<RunOrderingInfo>() == null))
            {
                var elementWithAnnoAfter = ce
                    .ElementsAfterSelf()
                    .FirstOrDefault(e => e.Annotation<RunOrderingInfo>() != null);
                if (elementWithAnnoAfter != null)
                {
                    int nextIndex = elementWithAnnoAfter.Annotation<RunOrderingInfo>().Integer;
                    var elementsAfter = ce.ElementsAfterSelf()
                        .TakeWhile(ea => ea.Annotation<RunOrderingInfo>() == null);
                    ce.AddAnnotation(new RunOrderingInfo(nextIndex - elementsAfter.Count() - 1));
                    continue;
                }
                // The element before must have an annotation.
                var elementBefore = ce.ElementsBeforeSelfReverseDocumentOrder().FirstOrDefault();
                if (elementBefore != null)
                {
                    RunOrderingInfo elementBeforeAnnotation = elementBefore.Annotation<RunOrderingInfo>();
                    ce.AddAnnotation(new RunOrderingInfo(elementBeforeAnnotation.Integer + 1));
                }
            }
        }

        private static XAttribute GetXmlSpaceAttribute(
            string textElementValue)
        {
            if (textElementValue.Length > 0 &&
                (textElementValue[0] == ' ' ||
                textElementValue[textElementValue.Length - 1] == ' '))
                return new XAttribute(XNamespace.Xml + "space",
                    "preserve");
            return null;
        }

        private static XElement MergeElementWithChildrenCommentElements(XElement e1, XElement e2, 
            XDocument commentsPartXDoc)
        {
            if (e1.Name.Namespace != W.w)
                Console.WriteLine("processing mathml");
            if (e1.Name != e2.Name)
                throw new CommentMergerInternalException(
                    "attempting to merge elements that do not match up.");

            var split1 = SplitElements(e1.Elements());
            var split2 = SplitElements(e2.Elements());
            XElement temp1 = new XElement(e1.Name,
                e1.Attributes(),
                split1);
            XElement temp2 = new XElement(e2.Name,
                e2.Attributes(),
                split2);

            // todo may want to remove following test.
            if (temp1.Elements().Where(e => e.Name == W.r || e.Name == M.r).Where(r =>
                {
                    foreach (var item in RunContentElements)
                        if (r.Element(item) != null)
                            return true;
                    return false;
                }).Count() !=
                temp2.Elements().Where(e => e.Name == W.r || e.Name == M.r).Where(r =>
                {
                    foreach (var item in RunContentElements)
                        if (r.Element(item) != null)
                            return true;
                    return false;
                }).Count())
                throw new CommentMergerInternalException("runs do not line up");

            AddAnnotationsToChildren(temp1);
            AddAnnotationsToChildren(temp2);

            if (temp1.Elements().Any(e => e.Annotation<RunOrderingInfo>() == null) ||
                temp2.Elements().Any(e => e.Annotation<RunOrderingInfo>() == null))
                throw new CommentMergerInternalException("some child does not have annotation");

            var m = temp1
                .Elements()
                .Concat(temp2.Elements())
                .OrderBy(e => e.Annotation<RunOrderingInfo>().Integer)
                .GroupAdjacent(e => e.Annotation<RunOrderingInfo>().Integer);

            XElement mergedElementWithSplitRuns = new XElement(e1.Name,
                e1.Attributes(),
                m.Select(g =>
                    {
                        if (g.First().Annotation<RunOrderingInfo>().IsRun)
                            return (object)g.First();
                        else
                            return g;
                    }));

            // The following query serves to remove duplicate comments, i.e. the same exact
            // comment from the same person is in each of the documents being merged.
            var groupedCommentReferences = mergedElementWithSplitRuns.Elements()
                    .GroupAdjacent(e =>
                        {
                            if (e.Name == W.r || e.Name == M.r)
                            {
                                bool onlyOneChild = e.Elements().Where(z => z.Name != W.rPr).Count() == 1;
                                if (onlyOneChild)
                                {
                                    XElement child = e.Elements().Where(z => z.Name != W.rPr).First();
                                    if (child.Name == W.commentRangeStart ||
                                        child.Name == W.commentRangeEnd ||
                                        child.Name == W.commentReference)
                                    {
                                        XElement comment = commentsPartXDoc.Root
                                            .Elements(W.comment)
                                            .Where(c => (string)c.Attribute(W.id) == (string)child.Attribute(W.id))
                                            .FirstOrDefault();
                                        string s = child.Name.LocalName + "|" + 
                                            comment.Attribute(W.author).Value + "|" +
                                            comment.Attribute(W.date).Value + "|" + 
                                            comment.Attribute(W.initials).Value + "|" +
                                            comment.Descendants()
                                                .Where(d => d.Name == W.t || d.Name == M.t)
                                                .Select(t => (string)t).StringConcatenate();
                                        return s;
                                    }
                                }
                            }
                            if (e.Name != W.commentRangeStart &&
                                e.Name != W.commentRangeEnd &&
                                e.Name != W.commentReference)
                                return "NotAComment";
                            XElement comment2 = commentsPartXDoc.Root
                                .Elements(W.comment)
                                .Where(c => (string)c.Attribute(W.id) == (string)e.Attribute(W.id))
                                .FirstOrDefault();
                            string s2 = e.Name.LocalName + "|" + 
                                comment2.Attribute(W.author).Value + "|" +
                                comment2.Attribute(W.date).Value + "|" + 
                                comment2.Attribute(W.initials).Value + "|" +
                                comment2.Descendants()
                                    .Where(d => d.Name == W.t || d.Name == M.t)
                                    .Select(t => (string)t).StringConcatenate();
                            return s2;
                        });

            XElement duplicateCommentsEliminated = new XElement(e1.Name,
                e1.Attributes(),
                    groupedCommentReferences
                    .Select(g =>
                        {
                            if (g.Key == "NotAComment")
                                return (object)g;
                            return g.First();
                        }));

            var runGroups = duplicateCommentsEliminated.Elements()
                .GroupAdjacent(r =>
                {
                    if (r.Name != W.r && r.Name != M.r)
                        return "NotRuns";
                    if (! r.Elements().Where(e => e.Name == W.rPr || e.Name == M.rPr).Any())
                        return "NoRunProperties";
                    XElement frag = new XElement("frag",
                        new XAttribute(XNamespace.Xmlns + "w", W.w),
                        r.Elements().Where(e => e.Name == W.rPr || e.Name == M.rPr));
                    return frag.ToString(
                        SaveOptions.DisableFormatting);
                });
            
            XElement newParagraph = new XElement(e1.Name,
                e1.Attributes(),
                runGroups.Select(g =>
                {
                    if (g.Key == "NotRuns")
                        return (object)g;
                    if (g.Key == "NoRunProperties")
                    {
                        XElement newRun = new XElement(g.First().Name,
                            g.First().Attributes(),
                            g.Elements()
                                .GroupAdjacent(c => c.Name)
                                .Select(gc =>
                                {
                                    if (gc.Key != W.t && gc.Key != M.t)
                                        return (object)gc;
                                    string textElementValue =
                                        gc.Select(t => (string)t)
                                            .StringConcatenate();
                                    return new XElement(gc.First().Name.Namespace + "t",
                                        GetXmlSpaceAttribute(
                                            textElementValue),
                                            textElementValue);
                                }));
                        return newRun;
                    }
                    XElement runPropertyFragment = XElement.Parse(
                        g.Key);
                    runPropertyFragment.Elements().Attributes()
                        .Where(a => a.IsNamespaceDeclaration)
                        .Remove();
                    XElement newRunWithProperties = new XElement(g.First().Name.Namespace + "r",
                        runPropertyFragment.Elements(),
                        g.Elements()
                            .Where(e => e.Name != W.rPr && e.Name != M.rPr)
                            .GroupAdjacent(c => c.Name)
                            .Select(gc =>
                            {
                                if (gc.Key != W.t && gc.Key != M.t)
                                    return (object)gc;
                                string textElementValue = gc
                                    .Select(t => (string)t)
                                    .StringConcatenate();
                                return new XElement(gc.Key,
                                    GetXmlSpaceAttribute(textElementValue),
                                    textElementValue);
                            }));
                    return newRunWithProperties;
                }
                ));

            return newParagraph;
        }

        private static object MergeElementTransform(XElement e1, XElement e2, XDocument commentsPartXDoc)
        {
            // todo need better message
            if (e1.Name != e2.Name)
                throw new CommentMergerDifferingContentsException("Internal error");
            if (e1.Elements()
                .Where(e => IsCommentElement(e)).Any() || e2.Elements().Where(e => IsCommentElement(e))
                .Any())
                return MergeElementWithChildrenCommentElements(e1, e2, commentsPartXDoc);
            var zippedChildren = e1.Elements().Zip(e2.Elements(), (p1, p2) => new
            {
                Element1 = p1,
                Element2 = p2,
            });
            return new XElement(e1.Name,
                e1.Attributes(),
                zippedChildren.Select(z =>
                    {
                        if (z.Element1.Name == W.t && z.Element2.Name == W.t &&
                            z.Element1.Value == z.Element2.Value)
                            return new XElement(W.t,
                                GetXmlSpaceAttribute(z.Element1.Value),
                                z.Element1.Value);
                        if (z.Element1.Name == M.t && z.Element2.Name == M.t &&
                            z.Element1.Value == z.Element2.Value)
                            return new XElement(M.t,
                                GetXmlSpaceAttribute(z.Element1.Value),
                                z.Element1.Value);
                        return MergeElementTransform(z.Element1, z.Element2, commentsPartXDoc);
                    }));
        }

        private static void MergeCommentsInPart(OpenXmlPart part1, OpenXmlPart part2, 
            OpenXmlPart destinationPart, XDocument commentsPartXDoc)
        {
            XDocument xdoc1 = part1.GetXDocument();
            XDocument xdoc2 = part2.GetXDocument();

            XElement newRootElement = (XElement)MergeElementTransform(xdoc1.Root, xdoc2.Root, 
                commentsPartXDoc);
            destinationPart.PutXDocument(new XDocument(newRootElement));
        }

        // todo are there any other elements that need ids fixed?  see children of paragraph, run
        private static void FixIdsInPart(OpenXmlPart part, int nextCommentId, int nextBookmarkId)
        {
            if (part == null)
                return;
            foreach (var element in part.GetXDocument().Root.Descendants()
                .Where(e => e.Name == W.commentRangeStart ||
                    e.Name == W.commentRangeEnd || 
                    e.Name == W.commentReference))
                element.Attribute(W.id).Value = ((int)element.Attribute(W.id) + nextCommentId).ToString();
            foreach (var element in part.GetXDocument().Root.Descendants()
                .Where(e => e.Name == W.bookmarkStart || e.Name == W.bookmarkEnd))
                element.Attribute(W.id).Value = ((int)element.Attribute(W.id) + nextBookmarkId).ToString();
        }

        private static void RenumberCommentsInDoc2(WordprocessingDocument doc1, WordprocessingDocument doc2)
        {
            // Get the XDocuments for the two comments parts.
            XDocument doc1WordprocessingCommentsXDocument = doc1
                .MainDocumentPart
                .WordprocessingCommentsPart
                .GetXDocument();
            int commentIdIncrease = doc1WordprocessingCommentsXDocument
                .Root
                .Elements(W.comment)
                .Attributes(W.id)
                .Select(a => (int)a)
                .Max() + 1;
            int nextBookmarkId = new[] { 0 }
                .Concat(doc1.MainDocumentPart
                    .GetXDocument()
                    .Root
                    .Descendants(W.bookmarkStart)
                    .Attributes(W.id)
                    .Select(a => (int)a))
                    .Max();
            foreach (var part in doc1.MainDocumentPart.HeaderParts)
                nextBookmarkId = new[] { nextBookmarkId }
                    .Concat(part
                        .GetXDocument()
                        .Root
                        .Descendants(W.bookmarkStart)
                        .Attributes(W.id)
                        .Select(a => (int)a))
                        .Max();
            foreach (var part in doc1.MainDocumentPart.FooterParts)
                nextBookmarkId = new[] { nextBookmarkId }
                    .Concat(part
                        .GetXDocument()
                        .Root
                        .Descendants(W.bookmarkStart)
                        .Attributes(W.id)
                        .Select(a => (int)a))
                        .Max();
            if (doc1.MainDocumentPart.FootnotesPart != null)
                nextBookmarkId = new[] { nextBookmarkId }
                    .Concat(doc1
                        .MainDocumentPart
                        .FootnotesPart
                        .GetXDocument()
                        .Root
                        .Descendants(W.bookmarkStart)
                        .Attributes(W.id)
                        .Select(a => (int)a))
                        .Max();
            if (doc1.MainDocumentPart.EndnotesPart != null)
                nextBookmarkId = new[] { nextBookmarkId }
                    .Concat(doc1
                        .MainDocumentPart
                        .EndnotesPart
                        .GetXDocument()
                        .Root
                        .Descendants(W.bookmarkStart)
                        .Attributes(W.id)
                        .Select(a => (int)a))
                        .Max();
            nextBookmarkId++;

            foreach (var element in doc2.MainDocumentPart.WordprocessingCommentsPart.GetXDocument()
                                        .Root.Elements(W.comment))
                element.Attribute(W.id).Value = 
                    (((int)element.Attribute(W.id)) + commentIdIncrease).ToString();
            FixIdsInPart(doc2.MainDocumentPart, commentIdIncrease, nextBookmarkId);
            foreach (var header in doc2.MainDocumentPart.HeaderParts)
                FixIdsInPart(header, commentIdIncrease, nextBookmarkId);
            foreach (var footer in doc2.MainDocumentPart.FooterParts)
                FixIdsInPart(footer, commentIdIncrease, nextBookmarkId);
            FixIdsInPart(doc2.MainDocumentPart.FootnotesPart, commentIdIncrease, nextBookmarkId);
            FixIdsInPart(doc2.MainDocumentPart.EndnotesPart, commentIdIncrease, nextBookmarkId);
        }
    }
}

#if false
todo here are two m:oMath elements that need merged.

<m:oMath xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math">
  <m:r>
    <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
      <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
    </w:rPr>
    <m:t>W</m:t>
  </m:r>
  <m:r>
    <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
      <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
    </w:rPr>
    <m:t>=</m:t>
  </m:r>
  <m:f>
    <m:fPr>
      <m:ctrlPr>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
          <w:i />
        </w:rPr>
      </m:ctrlPr>
    </m:fPr>
    <m:num>
      <m:r>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
        </w:rPr>
        <m:t>1</m:t>
      </m:r>
    </m:num>
    <m:den>
      <m:r>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
        </w:rPr>
        <m:t>μ(1-U)</m:t>
      </m:r>
    </m:den>
  </m:f>
</m:oMath>



<m:oMath xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math">
  <w:commentRangeStart w:id="6" xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main" />
  <m:r>
    <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
      <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
    </w:rPr>
    <m:t>W</m:t>
  </m:r>
  <w:commentRangeEnd w:id="6" xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main" />
  <m:r>
    <m:rPr>
      <m:sty m:val="p" />
    </m:rPr>
    <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
      <w:rStyle w:val="CommentReference" />
    </w:rPr>
    <w:commentReference w:id="6" xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main" />
  </m:r>
  <m:r>
    <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
      <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
    </w:rPr>
    <m:t>=</m:t>
  </m:r>
  <m:f>
    <m:fPr>
      <m:ctrlPr>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
          <w:i />
        </w:rPr>
      </m:ctrlPr>
    </m:fPr>
    <m:num>
      <m:r>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
        </w:rPr>
        <m:t>1</m:t>
      </m:r>
    </m:num>
    <m:den>
      <m:r>
        <w:rPr xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
          <w:rFonts w:ascii="Cambria Math" w:hAnsi="Cambria Math" />
        </w:rPr>
        <m:t>μ(1-U)</m:t>
      </m:r>
    </m:den>
  </m:f>
</m:oMath>
#endif
