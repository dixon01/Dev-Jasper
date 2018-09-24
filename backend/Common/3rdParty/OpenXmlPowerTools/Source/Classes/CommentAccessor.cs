/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public partial class WmlDocument : OpenXmlPowerToolsDocument
    {
        public object GetAllComments(CommentFormat format)
        {
            return CommentAccessor.GetAllComments(this, format);
        }
    }

    public enum CommentFormat
    {
        PlainText,
        Xml,
        Docx
    }

    /// <summary>
    /// Provides access to comment operations
    /// </summary>
    public class CommentAccessor
    {
        public static object GetAllComments(WmlDocument doc, CommentFormat format)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                IEnumerable<XElement> comments = null;
                WordprocessingCommentsPart commentsPart = document.MainDocumentPart.WordprocessingCommentsPart;
                if (commentsPart != null)
                {
                    XDocument commentsPartDocument = commentsPart.GetXDocument();
                    comments = commentsPartDocument.Element(W.comments).Elements(W.comment);
                }
                if (comments != null)
                {
                    XDocument commentsDocument =
                        new XDocument(
                            new XElement(W.comments,
                                new XAttribute(XNamespace.Xmlns + "w", W.w),
                                comments
                            )
                        );
                    switch (format)
                    {
                        case CommentFormat.PlainText:
                            return commentsDocument.ToString();
                        case CommentFormat.Xml:
                            return commentsDocument;
                        case CommentFormat.Docx:
                            return CreateCommentsDocument(comments);
                    }
                }
                return null;
            }
        }

        private static OpenXmlPowerToolsDocument CreateCommentsDocument(IEnumerable<XElement> contents)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = OpenXmlMemoryStreamDocument.CreateWordprocessingDocument())
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    PowerToolsExtensions.SetContent(document, contents);
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Returns all reference tags from inside the main part of a wordprocessing document
        /// </summary>
        private static IEnumerable<XElement> CommentReferences(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();

            IEnumerable<XElement> results =
                mainDocument.Descendants().Where(
                    tag =>
                        tag.Name == W.commentRangeStart ||
                        tag.Name == W.commentRangeEnd ||
                        (tag.Name == W.r && tag.Descendants(W.commentReference).Count() > 0)
                );
            return results;
        }

        /// <summary>
        /// Removes all of the comments existing in the document
        /// </summary>
        public static OpenXmlPowerToolsDocument RemoveAll(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    //Removes comment-related tags inside the main document part
                    IEnumerable<XElement> commentReferences = CommentReferences(document).ToList();
                    commentReferences.Remove();
                    document.MainDocumentPart.PutXDocument();

                    WordprocessingCommentsPart commentsPart = document.MainDocumentPart.WordprocessingCommentsPart;
                    if (commentsPart != null)
                        commentsPart.RemovePart();
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
