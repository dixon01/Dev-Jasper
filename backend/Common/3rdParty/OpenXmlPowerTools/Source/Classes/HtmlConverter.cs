/***************************************************************************

Copyright (c) Microsoft Corporation 2010.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license
can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public partial class WmlDocument : OpenXmlPowerToolsDocument
    {
        public XElement ConvertToHtml(HtmlConverterSettings htmlConverterSettings, Func<ImageInfo, XElement> imageHandler)
        {
            return HtmlConverter.ConvertToHtml(this, htmlConverterSettings, imageHandler);
        }
        public XElement ConvertToHtml(HtmlConverterSettings htmlConverterSettings)
        {
            return HtmlConverter.ConvertToHtml(this, htmlConverterSettings);
        }
    }

    public class HtmlConverterSettings
    {
        public string PageTitle;
        public string CssClassPrefix;
        public string Css;
        public bool ConvertFormatting;
    }

    public static class Xhtml
    {
        public static XNamespace xhtml = "http://www.w3.org/1999/xhtml";
        public static XName html = xhtml + "html";
        public static XName head = xhtml + "head";
        public static XName title = xhtml + "title";
        public static XName body = xhtml + "body";
        public static XName p = xhtml + "p";
        public static XName h1 = xhtml + "h1";
        public static XName h2 = xhtml + "h2";
        public static XName A = xhtml + "A";
        public static XName b = xhtml + "b";
        public static XName table = xhtml + "table";
        public static XName tr = xhtml + "tr";
        public static XName td = xhtml + "td";
        public static XName meta = xhtml + "meta";
        public static XName style = xhtml + "style";
        public static XName br = xhtml + "br";
        public static XName img = xhtml + "img";
        public static XName span = xhtml + "span";
    }

    public static class HtmlNoNamespace
    {
        public static XName href = "href";
        public static XName border = "border";
        public static XName http_equiv = "http-equiv";
        public static XName content = "content";
        public static XName name = "name";
        public static XName width = "width";
        public static XName height = "height";
        public static XName src = "src";
        public static XName style = "style";
        public static XName alt = "alt";
        public static XName id = "id";
        public static XName descr = "descr";
        public static XName _class = "class";
    }

    public class ImageInfo
    {
        public Bitmap Bitmap;
        public XAttribute ImgStyleAttribute;
        public string ContentType;
        public XElement DrawingElement;
        public string AltText;

        public static int EmusPerInch = 914400;
        public static int EmusPerCm = 360000;
    }

    public static class HtmlConverter
    {
        private static Dictionary<char, string> EntityMap = null;

        public class InvalidSettingsException : Exception
        {
            public InvalidSettingsException(string message) : base(message) { }
        }

        private static XElement ProcessImage(WordprocessingDocument wordDoc,
            XElement element, Func<ImageInfo, XElement> imageHandler)
        {
            if (element.Name == W.drawing)
            {
                XElement containerElement = element.Elements()
                    .Where(e => e.Name == WP.inline || e.Name == WP.anchor)
                    .FirstOrDefault();
                if (containerElement != null)
                {
                    int? extentCx = (int?)containerElement.Elements(WP.extent)
                        .Attributes(NoNamespace.cx).FirstOrDefault();
                    int? extentCy = (int?)containerElement.Elements(WP.extent)
                        .Attributes(NoNamespace.cy).FirstOrDefault();
                    string altText = (string)containerElement.Elements(WP.docPr)
                        .Attributes(NoNamespace.descr).FirstOrDefault();
                    if (altText == null)
                        altText = (string)containerElement.Elements(WP.docPr)
                            .Attributes(NoNamespace.name).FirstOrDefault();
                    if (altText == null)
                        altText = "";

                    XElement blipFill = containerElement.Elements(A.graphic)
                        .Elements(A.graphicData)
                        .Elements(Pic._pic).Elements(Pic.blipFill).FirstOrDefault();
                    if (blipFill != null)
                    {
                        string imageRid = (string)blipFill.Elements(A.blip).Attributes(R.embed)
                            .FirstOrDefault();
                        ImagePart imagePart = (ImagePart)wordDoc.MainDocumentPart
                            .GetPartById(imageRid);
                        string contentType = imagePart.ContentType;
                        if (contentType == "image/png" ||
                            contentType == "image/gif" ||
                            contentType == "image/tiff" ||
                            contentType == "image/jpeg")
                        {
                            using (Stream partStream = imagePart.GetStream())
                            using (Bitmap bitmap = new Bitmap(partStream))
                            {
                                if (extentCx != null && extentCy != null)
                                {
                                    ImageInfo imageInfo = new ImageInfo()
                                    {
                                        Bitmap = bitmap,
                                        ImgStyleAttribute = new XAttribute(HtmlNoNamespace.style,
                                            string.Format("width: {0}in; height: {1}in",
                                                (float)extentCx / (float)ImageInfo.EmusPerInch,
                                                (float)extentCy / (float)ImageInfo.EmusPerInch)),
                                        ContentType = contentType,
                                        DrawingElement = element,
                                        AltText = altText,
                                    };
                                    return imageHandler(imageInfo);
                                }
                                ImageInfo imageInfo2 = new ImageInfo()
                                {
                                    Bitmap = bitmap,
                                    ContentType = contentType,
                                    DrawingElement = element,
                                    AltText = altText,
                                };
                                return imageHandler(imageInfo2);
                            };
                        }
                    }
                }
            }
            if (element.Name == W.pict)
            {
                string imageRid = (string)element.Elements(VML.shape)
                    .Elements(VML.imagedata).Attributes(R.id).FirstOrDefault();
                string style = (string)element.Elements(VML.shape)
                    .Attributes(HtmlNoNamespace.style).FirstOrDefault();
                if (imageRid != null)
                {
                    try
                    {
                        ImagePart imagePart = (ImagePart)wordDoc.MainDocumentPart
                            .GetPartById(imageRid);
                        string contentType = imagePart.ContentType;
                        if (contentType == "image/png" ||
                            contentType == "image/gif" ||
                            contentType == "image/tiff" ||
                            contentType == "image/jpeg")
                        {
                            //string style = element.
                            using (Stream partStream = imagePart.GetStream())
                            using (Bitmap bitmap = new Bitmap(partStream))
                            {
                                ImageInfo imageInfo = new ImageInfo()
                                {
                                    Bitmap = bitmap,
                                    ContentType = contentType,
                                    DrawingElement = element,
                                };
                                if (style != null)
                                {
                                    float? widthInPoints = null;
                                    float? heightInPoints = null;
                                    string[] tokens = style.Split(';');
                                    var widthString = tokens
                                        .Select(t => new
                                        {
                                            Name = t.Split(':').First(),
                                            Value = t.Split(':').Skip(1)
                                                .Take(1).FirstOrDefault(),
                                        })
                                        .Where(p => p.Name == "width")
                                        .Select(p => p.Value)
                                        .FirstOrDefault();
                                    if (widthString != null &&
                                        widthString.Substring(widthString.Length - 2) == "pt")
                                    {
                                        float w;
                                        if (float.TryParse(widthString.Substring(0,
                                            widthString.Length - 2), out w))
                                            widthInPoints = w;
                                    }
                                    var heightString = tokens
                                        .Select(t => new
                                        {
                                            Name = t.Split(':').First(),
                                            Value = t.Split(':').Skip(1).Take(1).FirstOrDefault(),
                                        })
                                        .Where(p => p.Name == "height")
                                        .Select(p => p.Value)
                                        .FirstOrDefault();
                                    if (heightString != null &&
                                        heightString.Substring(heightString.Length - 2) == "pt")
                                    {
                                        float h;
                                        if (float.TryParse(heightString.Substring(0,
                                            heightString.Length - 2), out h))
                                            heightInPoints = h;
                                    }
                                    if (widthInPoints != null && heightInPoints != null)
                                        imageInfo.ImgStyleAttribute = new XAttribute(
                                            HtmlNoNamespace.style, string.Format(
                                                "width: {0}pt; height: {1}pt",
                                                widthInPoints, heightInPoints));
                                }
                                return imageHandler(imageInfo);
                            };
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        private static object ConvertEntities(string text)
        {
            if (text == null)
                return null;
            object o = text.ToCharArray()
                .GroupAdjacent((char c) =>
                {
                    if (c == 0xf0b7 ||
                        c == 0xf0a7 ||
                        c == 0xf076 ||
                        c == 0xf0d8 ||
                        c == 0xf0a8 ||
                        c == 0xf0fc ||
                        c == 0xf0e0 ||
                        c == 0xf0b2)
                        return "bull";
                    if (c >= 0xf000)
                        return "loz";
                    if (c >= 128)
                    {
                        string entity;
                        if (EntityMap.TryGetValue(c, out entity))
                            return entity;
                    }
                    return "-";
                })
                .Select(g =>
                {
                    if (g.Key != "-")
                        return (object)(g.Select(c => new XEntity(g.Key)));
                    return new XText(g.Aggregate(new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString()));
                });
            return o;
        }

        private static object ConvertToHtmlTransform(WordprocessingDocument wordDoc,
            HtmlConverterSettings settings, XNode node,
            Func<ImageInfo, XElement> imageHandler)
        {
            XElement element = node as XElement;
            if (element != null)
            {
                if (element.Name == W.document)
                    return new XElement(Xhtml.html,
                        new XElement(Xhtml.head,
                            new XElement(Xhtml.meta,
                                new XAttribute(HtmlNoNamespace.http_equiv, "Content-Type"),
                                new XAttribute(HtmlNoNamespace.content,
                                    "text/html; charset=windows-1252")),
                            new XElement(Xhtml.meta,
                                new XAttribute(HtmlNoNamespace.name, "Generator"),
                                new XAttribute(HtmlNoNamespace.content,
                                    "PowerTools for Open XML")),
                            settings.PageTitle != null ? new XElement(Xhtml.title,
                                settings.PageTitle) : null,
                            settings.Css != null ? new XElement(Xhtml.style,
                                new XComment(Environment.NewLine +
                                    settings.Css + Environment.NewLine)) : null
                        ),
                        element.Elements().Select(e => ConvertToHtmlTransform(
                            wordDoc, settings, e, imageHandler))
                    );

                // Transform the w:body element to the XHTML h:body element.
                if (element.Name == W.body)
                    return new XElement(Xhtml.body,
                        element.Elements().Select(e => ConvertToHtmlTransform(
                            wordDoc, settings, e, imageHandler)));

                // Transform every paragraph with a style that has paragraph properties
                // that has an outline level into the same level of heading.  This takes
                // care of transforming headings of every level.
                if (element.Name == W.p)
                {
                    string styleId = (string)element.Elements(W.pPr).Elements(W.pStyle)
                        .Attributes(W.val).FirstOrDefault();
                    XElement style = wordDoc.MainDocumentPart.StyleDefinitionsPart
                        .GetXDocument().Root.Elements(W.style)
                        .Where(s => (string)s.Attribute(W.styleId) == styleId)
                        .FirstOrDefault();
                    if (style != null)
                    {
                        int? outlineLevel = (int?)style.Elements(W.pPr)
                            .Elements(W.outlineLvl).Attributes(W.val).FirstOrDefault();
                        if (outlineLevel != null)
                        {
                            return new XElement(Xhtml.xhtml + string.Format("h{0}",
                                outlineLevel + 1),
                                settings.CssClassPrefix != null ?
                                    new XAttribute(HtmlNoNamespace._class,
                                        settings.CssClassPrefix + styleId) : null,
                                ConvertEntities(ListItemRetriever.RetrieveListItem(wordDoc,
                                    element, null)),
                                element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                                    settings, e, imageHandler)));
                        }
                    }
                }
                
                // Transform w:p to h:p.
                if (element.Name == W.p)
                {
                    string styleId = (string)element.Elements(W.pPr).Elements(W.pStyle)
                        .Attributes(W.val).FirstOrDefault();
                    if (styleId == null)
                    {
                        styleId = (string)wordDoc.MainDocumentPart.StyleDefinitionsPart
                            .GetXDocument().Root.Elements(W.style)
                            .Where(e => (string)e.Attribute(W.type) == "paragraph" &&
                               (string)e.Attribute(W._default) == "1")
                            .FirstOrDefault().Attributes(W.styleId).FirstOrDefault();
                    }
                    XElement z = new XElement(Xhtml.p,
                        styleId != null ? (
                            settings.CssClassPrefix != null ?
                            new XAttribute(HtmlNoNamespace._class,
                                settings.CssClassPrefix + styleId) : null
                        ) : null,
                        ConvertEntities(ListItemRetriever.RetrieveListItem(wordDoc,
                            element, null)),
                        element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                            settings, e, imageHandler)));
                    return z;
                }

                // Transform every hyperlink in the document to the XHTML h:A element.
                if (element.Name == W.hyperlink && element.Attribute(R.id) != null)
                {
                    try
                    {
                        return new XElement(Xhtml.A,
                            new XAttribute(HtmlNoNamespace.href,
                                wordDoc.MainDocumentPart
                                    .HyperlinkRelationships
                                    .Where(x => x.Id == (string)element.Attribute(R.id))
                                    .First()
                                    .Uri
                            ),
                            ConvertEntities(element.Elements(W.r)
                                          .Elements(W.t)
                                          .Select(s => (string)s).StringConcatenate())
                        );
                    }
                    catch (UriFormatException)
                    {
                        return element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                            settings, e, imageHandler));
                    }
                }

                // Transform contents of runs that are part of a hyperlink.
                if (element.Name == W.r &&
                    element.Annotation<FieldInfo>() != null &&
                    element.Annotation<FieldInfo>().Arguments.Length > 0)
                {
                    FieldInfo fieldInfo = element.Annotation<FieldInfo>();
                    return new XElement(Xhtml.A,
                        new XAttribute(HtmlNoNamespace.href, fieldInfo.Arguments[0]),
                        ConvertEntities(element.Elements(W.t)
                            .Select(s => (string)s).StringConcatenate())
                    );
                }

                // Transform contents of runs.
                if (element.Name == W.r)
                    return element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                        settings, e, imageHandler));

                // Transform every w:t element to a text node.
                if (element.Name == W.t)
                    return ConvertEntities(element.Value);

                // Transform w:br to h:br.
                if (element.Name == W.br || element.Name == W.cr)
                    return new XElement(Xhtml.br);

                // Transform w:noBreakHyphen to '-'
                if (element.Name == W.noBreakHyphen)
                    return new XText("-");

                // Transform w:tbl to h:tbl.
                if (element.Name == W.tbl)
                    return new XElement(Xhtml.table,
                        new XAttribute(HtmlNoNamespace.border, 1),
                        element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                            settings, e, imageHandler)));

                // Transform w:tr to h:tr.
                if (element.Name == W.tr)
                    return new XElement(Xhtml.tr,
                        element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                            settings, e, imageHandler)));

                // Transform w:tc to h:td.
                if (element.Name == W.tc)
                    return new XElement(Xhtml.td,
                        element.Elements().Select(e => ConvertToHtmlTransform(wordDoc,
                            settings, e, imageHandler)));

                // Transform images.
                if (element.Name == W.drawing || element.Name == W.pict)
                {
                    if (imageHandler == null)
                        return null;
                    return ProcessImage(wordDoc, element, imageHandler);
                }

                // The following removes any nodes that haven't been transformed.
                return null;
            }
            return null;
        }

        private enum AnnotateState
        {
            NotInHyperlink,
            InFirstSection,
            InSecondSection,
        }

        private static void AnnotateHyperlinkContent(XElement rootElement)
        {
            AnnotateState state = AnnotateState.NotInHyperlink;
            foreach (XElement blockLevelContentContainer in
                rootElement.Descendants().Where(e => W.BlockLevelContentContainers.Contains(e.Name)))
            {
                FieldInfo fieldInfo = null;
                foreach (XElement runLevelContent in blockLevelContentContainer
                    .LogicalChildrenContent(W.p).LogicalChildrenContent(W.r))
                {
                    if (runLevelContent.Elements(W.fldChar).Attributes(W.fldCharType)
                        .Any(a => a.Value == "begin"))
                        state = AnnotateState.InFirstSection;
                    XElement instrText = runLevelContent.Elements(W.instrText).FirstOrDefault();
                    if (instrText != null && state == AnnotateState.InFirstSection)
                    {
                        FieldInfo tempFieldInfo = FieldParser.ParseField(instrText.Value);
                        if (tempFieldInfo.FieldType == "HYPERLINK")
                            fieldInfo = tempFieldInfo;
                    }
                    var z = runLevelContent.Elements(W.fldChar).FirstOrDefault();
                    if (runLevelContent.Elements(W.fldChar).Attributes(W.fldCharType)
                        .Any(a => a.Value == "separate"))
                        state = AnnotateState.InSecondSection;
                    if (runLevelContent.Elements(W.fldChar).Attributes(W.fldCharType)
                        .Any(a => a.Value == "end"))
                    {
                        fieldInfo = null;
                        state = AnnotateState.NotInHyperlink;
                    }
                    if (state == AnnotateState.InSecondSection && fieldInfo != null &&
                        (string)runLevelContent.Elements(W.rPr).Elements(W.rStyle)
                        .Attributes(W.val).FirstOrDefault() == "Hyperlink")
                        runLevelContent.AddAnnotation(fieldInfo);
                }
            }
        }

        private static void InitEntityMap()
        {
            EntityMap = new Dictionary<char, string>()
            {
                { (char)160, "nbsp" },
                { (char)161, "iexcl" },
                { (char)162, "cent" },
                { (char)163, "pound" },
                { (char)164, "curren" },
                { (char)165, "yen" },
                { (char)166, "brvbar" },
                { (char)167, "sect" },
                { (char)168, "uml" },
                { (char)169, "copy" },
                { (char)170, "ordf" },
                { (char)171, "laquo" },
                { (char)172, "not" },
                { (char)173, "shy" },
                { (char)174, "reg" },
                { (char)175, "macr" },
                { (char)176, "deg" },
                { (char)177, "plusmn" },
                { (char)178, "sup2" },
                { (char)179, "sup3" },
                { (char)180, "acute" },
                { (char)181, "micro" },
                { (char)182, "para" },
                { (char)183, "middot" },
                { (char)184, "cedil" },
                { (char)185, "sup1" },
                { (char)186, "ordm" },
                { (char)187, "raquo" },
                { (char)188, "frac14" },
                { (char)189, "frac12" },
                { (char)190, "frac34" },
                { (char)191, "iquest" },
                { (char)192, "Agrave" },
                { (char)193, "Aacute" },
                { (char)194, "Acirc" },
                { (char)195, "Atilde" },
                { (char)196, "Auml" },
                { (char)197, "Aring" },
                { (char)198, "AElig" },
                { (char)199, "Ccedil" },
                { (char)200, "Egrave" },
                { (char)201, "Eacute" },
                { (char)202, "Ecirc" },
                { (char)203, "Euml" },
                { (char)204, "Igrave" },
                { (char)205, "Iacute" },
                { (char)206, "Icirc" },
                { (char)207, "Iuml" },
                { (char)208, "ETH" },
                { (char)209, "Ntilde" },
                { (char)210, "Ograve" },
                { (char)211, "Oacute" },
                { (char)212, "Ocirc" },
                { (char)213, "Otilde" },
                { (char)214, "Ouml" },
                { (char)215, "times" },
                { (char)216, "Oslash" },
                { (char)217, "Ugrave" },
                { (char)218, "Uacute" },
                { (char)219, "Ucirc" },
                { (char)220, "Uuml" },
                { (char)221, "Yacute" },
                { (char)222, "THORN" },
                { (char)223, "szlig" },
                { (char)224, "agrave" },
                { (char)225, "aacute" },
                { (char)226, "acirc" },
                { (char)227, "atilde" },
                { (char)228, "auml" },
                { (char)229, "aring" },
                { (char)230, "aelig" },
                { (char)231, "ccedil" },
                { (char)232, "egrave" },
                { (char)233, "eacute" },
                { (char)234, "ecirc" },
                { (char)235, "euml" },
                { (char)236, "igrave" },
                { (char)237, "iacute" },
                { (char)238, "icirc" },
                { (char)239, "iuml" },
                { (char)240, "eth" },
                { (char)241, "ntilde" },
                { (char)242, "ograve" },
                { (char)243, "oacute" },
                { (char)244, "ocirc" },
                { (char)245, "otilde" },
                { (char)246, "ouml" },
                { (char)247, "divide" },
                { (char)248, "oslash" },
                { (char)249, "ugrave" },
                { (char)250, "uacute" },
                { (char)251, "ucirc" },
                { (char)252, "uuml" },
                { (char)253, "yacute" },
                { (char)254, "thorn" },
                { (char)255, "yuml" },
                { (char)338, "OElig" },
                { (char)339, "oelig" },
                { (char)352, "Scaron" },
                { (char)353, "scaron" },
                { (char)376, "Yuml" },
                { (char)402, "fnof" },
                { (char)710, "circ" },
                { (char)732, "tilde" },
                { (char)913, "Alpha" },
                { (char)914, "Beta" },
                { (char)915, "Gamma" },
                { (char)916, "Delta" },
                { (char)917, "Epsilon" },
                { (char)918, "Zeta" },
                { (char)919, "Eta" },
                { (char)920, "Theta" },
                { (char)921, "Iota" },
                { (char)922, "Kappa" },
                { (char)923, "Lambda" },
                { (char)924, "Mu" },
                { (char)925, "Nu" },
                { (char)926, "Xi" },
                { (char)927, "Omicron" },
                { (char)928, "Pi" },
                { (char)929, "Rho" },
                { (char)931, "Sigma" },
                { (char)932, "Tau" },
                { (char)933, "Upsilon" },
                { (char)934, "Phi" },
                { (char)935, "Chi" },
                { (char)936, "Psi" },
                { (char)937, "Omega" },
                { (char)945, "alpha" },
                { (char)946, "beta" },
                { (char)947, "gamma" },
                { (char)948, "delta" },
                { (char)949, "epsilon" },
                { (char)950, "zeta" },
                { (char)951, "eta" },
                { (char)952, "theta" },
                { (char)953, "iota" },
                { (char)954, "kappa" },
                { (char)955, "lambda" },
                { (char)956, "mu" },
                { (char)957, "nu" },
                { (char)958, "xi" },
                { (char)959, "omicron" },
                { (char)960, "pi" },
                { (char)961, "rho" },
                { (char)962, "sigmaf" },
                { (char)963, "sigma" },
                { (char)964, "tau" },
                { (char)965, "upsilon" },
                { (char)966, "phi" },
                { (char)967, "chi" },
                { (char)968, "psi" },
                { (char)969, "omega" },
                { (char)977, "thetasym" },
                { (char)978, "upsih" },
                { (char)982, "piv" },
                { (char)8194, "ensp" },
                { (char)8195, "emsp" },
                { (char)8201, "thinsp" },
                { (char)8204, "zwnj" },
                { (char)8205, "zwj" },
                { (char)8206, "lrm" },
                { (char)8207, "rlm" },
                { (char)8211, "ndash" },
                { (char)8212, "mdash" },
                { (char)8216, "lsquo" },
                { (char)8217, "rsquo" },
                { (char)8218, "sbquo" },
                { (char)8220, "ldquo" },
                { (char)8221, "rdquo" },
                { (char)8222, "bdquo" },
                { (char)8224, "dagger" },
                { (char)8225, "Dagger" },
                { (char)8226, "bull" },
                { (char)8230, "hellip" },
                { (char)8240, "permil" },
                { (char)8242, "prime" },
                { (char)8243, "Prime" },
                { (char)8249, "lsaquo" },
                { (char)8250, "rsaquo" },
                { (char)8254, "oline" },
                { (char)8260, "frasl" },
                { (char)8364, "euro" },
                { (char)8465, "image" },
                { (char)8472, "weierp" },
                { (char)8476, "real" },
                { (char)8482, "trade" },
                { (char)8501, "alefsym" },
                { (char)8592, "larr" },
                { (char)8593, "uarr" },
                { (char)8594, "rarr" },
                { (char)8595, "darr" },
                { (char)8596, "harr" },
                { (char)8629, "crarr" },
                { (char)8656, "lArr" },
                { (char)8657, "uArr" },
                { (char)8658, "rArr" },
                { (char)8659, "dArr" },
                { (char)8660, "hArr" },
                { (char)8704, "forall" },
                { (char)8706, "part" },
                { (char)8707, "exist" },
                { (char)8709, "empty" },
                { (char)8711, "nabla" },
                { (char)8712, "isin" },
                { (char)8713, "notin" },
                { (char)8715, "ni" },
                { (char)8719, "prod" },
                { (char)8721, "sum" },
                { (char)8722, "minus" },
                { (char)8727, "lowast" },
                { (char)8730, "radic" },
                { (char)8733, "prop" },
                { (char)8734, "infin" },
                { (char)8736, "ang" },
                { (char)8743, "and" },
                { (char)8744, "or" },
                { (char)8745, "cap" },
                { (char)8746, "cup" },
                { (char)8747, "int" },
                { (char)8756, "there4" },
                { (char)8764, "sim" },
                { (char)8773, "cong" },
                { (char)8776, "asymp" },
                { (char)8800, "ne" },
                { (char)8801, "equiv" },
                { (char)8804, "le" },
                { (char)8805, "ge" },
                { (char)8834, "sub" },
                { (char)8835, "sup" },
                { (char)8836, "nsub" },
                { (char)8838, "sube" },
                { (char)8839, "supe" },
                { (char)8853, "oplus" },
                { (char)8855, "otimes" },
                { (char)8869, "perp" },
                { (char)8901, "sdot" },
                { (char)8968, "lceil" },
                { (char)8969, "rceil" },
                { (char)8970, "lfloor" },
                { (char)8971, "rfloor" },
                { (char)9001, "lang" },
                { (char)9002, "rang" },
                { (char)9674, "loz" },
                { (char)9824, "spades" },
                { (char)9827, "clubs" },
                { (char)9829, "hearts" },
                { (char)9830, "diams" },
            };
        }

        public static XElement ConvertToHtml(WordprocessingDocument wordDoc,
            HtmlConverterSettings htmlConverterSettings)
        {
            return ConvertToHtml(wordDoc, htmlConverterSettings, null);
        }

        public static XElement ConvertToHtml(WmlDocument doc, HtmlConverterSettings htmlConverterSettings)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    return ConvertToHtml(document, htmlConverterSettings);
                }
            }
        }

        public static XElement ConvertToHtml(WmlDocument doc, HtmlConverterSettings htmlConverterSettings, Func<ImageInfo, XElement> imageHandler)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    return ConvertToHtml(document, htmlConverterSettings, imageHandler);
                }
            }
        }

        public static XElement ConvertToHtml(WordprocessingDocument wordDoc,
            HtmlConverterSettings htmlConverterSettings, Func<ImageInfo, XElement> imageHandler)
        {
            InitEntityMap();
            if (htmlConverterSettings.ConvertFormatting)
            {
                throw new InvalidSettingsException("Conversion with formatting is not supported");
            }
            RevisionAccepter.AcceptRevisions(wordDoc);
            SimplifyMarkupSettings settings = new SimplifyMarkupSettings
            {
                RemoveComments = true,
                RemoveContentControls = true,
                RemoveEndAndFootNotes = true,
                RemoveFieldCodes = false,
                RemoveLastRenderedPageBreak = true,
                RemovePermissions = true,
                RemoveProof = true,
                RemoveRsidInfo = true,
                RemoveSmartTags = true,
                RemoveSoftHyphens = true,
                ReplaceTabsWithSpaces = true,
            };
            MarkupSimplifier.SimplifyMarkup(wordDoc, settings);
            XElement rootElement = wordDoc.MainDocumentPart.GetXDocument().Root;
            AnnotateHyperlinkContent(rootElement);
            XElement xhtml = (XElement)ConvertToHtmlTransform(wordDoc, htmlConverterSettings,
                rootElement, imageHandler);

            // Note: the xhtml returned by ConvertToHtmlTransform contains objects of type
            // XEntity.  PtOpenXmlUtil.cs define the XEntity class.  See
            // http://blogs.msdn.com/ericwhite/archive/2010/01/21/writing-entity-references-using-linq-to-xml.aspx
            // for detailed explanation.
            //
            // If you further transform the XML tree returned by ConvertToHtmlTransform, you
            // must do it correctly, or entities will not be serialized properly.

            return xhtml;
        }
    }
}
