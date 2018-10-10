/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Manages WordprocessingDocument content
    /// </summary>
    public class WordprocessingDocumentManager
    {
        private static XNamespace ns;
        private static XNamespace relationshipsns;
        private const string ValAttrName = "w:val";

        static WordprocessingDocumentManager()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            relationshipsns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        }

        /// <summary>
        /// Creates the xml of a main document part content, with an empty body.
        /// </summary>
        /// <returns>the XDocument result</returns>
        private static XDocument CreateMainDocumentPartXml()
        {
            return new XDocument(
                new XElement(ns + "document",
                    new XAttribute(XNamespace.Xmlns + "w", ns),
                    new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                    new XElement(ns + "body")
                )
            );
        }

        /// <summary>
        /// Creates the xml of a main document part content, adding some text inside
        /// </summary>
        /// <param name="text">Text to add inside the document</param>
        private static XDocument CreateMainDocumentPartXml(string text)
        {
            return new XDocument(
                new XElement(ns + "document",
                    new XAttribute(XNamespace.Xmlns + "w", ns),
                    new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                    new XElement(ns + "body",
                        new XElement(ns + "p",
                            new XElement(ns + "r",
                                new XElement(ns + "t", text)
                            )
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Creates the Xml of a main document part content, adding inside the body element a group of xml elements
        /// </summary>
        /// <param name="contents">XElement list with contents to add inside a document</param>
        /// <returns>XDocument result</returns>
        private static XDocument CreateMainDocumentPartXml(IEnumerable<XElement> contents)
        {
            return new XDocument(
                new XElement(ns + "document",
                    new XAttribute(XNamespace.Xmlns + "w", ns),
                    new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                    new XElement(ns + "body", 
                        contents
                    )
                )
            );
        }

        private static void LoadRelatedPart(XmlDocument mainDoc, XmlNode node, Stream stream)
        {
            XmlDocument partDoc = new XmlDocument();
            partDoc.Load(stream);
            XmlNode partNode = mainDoc.ImportNode(partDoc.DocumentElement, true);
            if (partNode != null)
                node.AppendChild(partNode);
        }
        private static XmlNamespaceManager createNameSpaceManager(XmlNameTable nameTable)
        {
            XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(nameTable);

            nameSpaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            nameSpaceManager.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            nameSpaceManager.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            nameSpaceManager.AddNamespace("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");
            nameSpaceManager.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");

            nameSpaceManager.AddNamespace("v", "urn:schemas-microsoft-com:vml");
            nameSpaceManager.AddNamespace("w10", "urn:schemas-microsoft-com:office:word");
            nameSpaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");

            return nameSpaceManager;
        }

#if false
        private class ListLevel
        {
            /// <summary>
            /// constructor used when abstract list levels are instantiated
            /// </summary>
            public ListLevel(XmlNode levelNode, XmlNamespaceManager nsm)
            {
                this.id = getAttributeValue(levelNode, "w:ilvl");

                XmlNode startValueNode = levelNode.SelectSingleNode("w:start", nsm);

                if (startValueNode != null)
                {
                    string startValueString = getAttributeValue(startValueNode, ValAttrName);

                    if (!String.IsNullOrEmpty(startValueString))
                    {
                        this.startValue = System.Convert.ToUInt32(startValueString, CultureInfo.InvariantCulture) - 1; // as I have to increment counters before each instance (to keep sub-list numbering from advancing too early, set startValue one lower;
                    }

                }
                this.started = false;

                XmlNode levelTextNode = levelNode.SelectSingleNode("w:lvlText", nsm);

                if (levelTextNode != null)
                {
                    this.levelText = getAttributeValue(levelTextNode, ValAttrName);
                }

                XmlNode fontNode = levelNode.SelectSingleNode(".//w:rFonts", nsm);

                if (fontNode != null)
                {
                    this.font = getAttributeValue(fontNode, "w:hAnsi");
                }

                XmlNode enumTypeNode = levelNode.SelectSingleNode("w:numFmt", nsm);

                if (enumTypeNode != null)
                {
                    string type = getAttributeValue(enumTypeNode, ValAttrName);

                    // w:numFmt="bullet" indicates a bulleted list
                    this.isBullet = String.Compare(type, "bullet", StringComparison.OrdinalIgnoreCase) == 0;
                    // w:numFmt="lowerLetter" indicates letter conversion instead of number
                    this.isUpperLetter = String.Compare(type, "upperLetter", StringComparison.OrdinalIgnoreCase) == 0;
                }

            }

            /// <summary>
            /// copy constructor
            /// </summary>
            /// <param name="masterCopy"></param>
            public ListLevel(ListLevel masterCopy)
            {
                this.abstractLevel = masterCopy;
                this.id = masterCopy.ID;
                this.levelText = masterCopy.LevelText;
                this.startValue = masterCopy.StartValue;
                this.started = false;
                this.font = masterCopy.Font;
                this.isBullet = masterCopy.IsBullet;
                this.isUpperLetter = masterCopy.IsUpperLetter;
            }

            private ListLevel abstractLevel;

            /// <summary>
            /// Get overridden values
            /// </summary>
            /// <param name="levelNode"></param>
            /// <param name="nsm"></param>
            public void SetOverrides(XmlNode levelNode, XmlNamespaceManager nsm)
            {
                XmlNode startValueNode = levelNode.SelectSingleNode("w:start", nsm);

                if (startValueNode != null)
                {
                    string startValueString = getAttributeValue(startValueNode, ValAttrName);
                    this.startValue = System.Convert.ToUInt32(startValueString, CultureInfo.InvariantCulture) - 1; // as I have to increment counters before each instance (to keep sub-list numbering from advancing too early, set startValue one lower
                }

                XmlNode levelTextNode = levelNode.SelectSingleNode("w:lvlText", nsm);

                if (levelTextNode != null)
                {
                    this.levelText = getAttributeValue(levelTextNode, ValAttrName);
                }

                XmlNode fontNode = levelNode.SelectSingleNode("//w:rFonts", nsm);

                if (fontNode != null)
                {
                    this.font = getAttributeValue(fontNode, "w:hAnsi");
                }

                XmlNode enumTypeNode = levelNode.SelectSingleNode("w:numFmt", nsm);

                if (enumTypeNode != null)
                {
                    string type = getAttributeValue(enumTypeNode, ValAttrName);

                    // w:numFmt="bullet" indicates a bulleted list
                    this.isBullet = String.Compare(type, "bullet", StringComparison.OrdinalIgnoreCase) == 0;
                }
            }

            public string FormatValue()
            {
                if (isUpperLetter)
                    return Convert.ToChar(('A' + CurrentValue - 1)).ToString(CultureInfo.InvariantCulture);
                return CurrentValue.ToString(CultureInfo.InvariantCulture);
            }

            private string id;

            /// <summary>
            /// returns the ID of the level
            /// </summary>
            public string ID
            {
                get
                {
                    return this.id;
                }
            }

            private UInt32 startValue;
            private bool started;

            /// <summary>
            /// start value of that level
            /// </summary>
            public UInt32 StartValue
            {
                get
                {
                    return this.startValue;
                }
            }

            private UInt32 counter;

            /// <summary>
            /// returns the current count of list items of that level
            /// </summary>
            public UInt32 CurrentValue
            {
                get
                {
                    if (this.abstractLevel != null)
                        return this.abstractLevel.CurrentValue;
                    return this.counter;
                }
            }

            /// <summary>
            /// increments the current count of list items of that level
            /// </summary>
            public void IncrementCounter()
            {
                if (!started)
                {
                    ResetCounter();
                    this.started = true;
                }
                if (this.abstractLevel != null)
                    this.abstractLevel.counter++;
                else
                    this.counter++;
            }

            /// <summary>
            /// resets the counter to the start value
            /// </summary>
            /// <id guid="823b5a3c-7501-4746-8dc4-7b098de5947a" />
            /// <owner alias="ROrleth" />
            public void ResetCounter()
            {
                if (this.abstractLevel != null)
                    this.abstractLevel.counter = this.startValue;
                else
                    this.counter = this.startValue;
            }

            private string levelText;

            /// <summary>
            /// returns the indicated lvlText value
            /// </summary>
            public string LevelText
            {
                get
                {
                    return this.levelText;
                }
            }

            private string font;

            /// <summary>
            /// returns the font name
            /// </summary>
            public string Font
            {
                get
                {
                    return this.font;
                }
            }

            private bool isBullet;

            /// <summary>
            /// returns whether the enumeration type is a bulleted list or not
            /// </summary>
            public bool IsBullet
            {
                get
                {
                    return this.isBullet;
                }
            }

            private bool isUpperLetter;

            /// <summary>
            /// returns whether the enumeration type is upper-case letter or not
            /// </summary>
            public bool IsUpperLetter
            {
                get { return this.isUpperLetter; }
            }
        }
        
        /// <summary>
        /// private helper class to deal with abstract number lists
        /// </summary>
        private class AbstractListNumberingDefinition
        {
            private Dictionary<string, ListLevel> listLevels;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="abstractNumNode"></param>
            /// <param name="nsm"></param>
            public AbstractListNumberingDefinition(XmlNode abstractNumNode, XmlNamespaceManager nsm)
            {
                string abstractNumString = getAttributeValue(abstractNumNode, "w:abstractNumId");

                if (!String.IsNullOrEmpty(abstractNumString))
                {
                    this.abstractNumDefId = abstractNumString;

                    this.readListLevelsFromAbsNode(abstractNumNode, nsm);

                    // find out whether there is a linked abstractNum definition that this needs to be populated from later on
                    XmlNode linkedStyleNode = abstractNumNode.SelectSingleNode("./w:numStyleLink", nsm);

                    if (linkedStyleNode != null)
                    {
                        this.linkedStyleId = getAttributeValue(linkedStyleNode, ValAttrName);
                    }
                }
            }

            /// <summary>
            /// update the level definitions from a linked abstractNum node
            /// </summary>
            /// <param name="linkedNode">
            /// </param>
            /// <param name="nsm">
            /// </param>
            /// <id guid="36473168-7947-41ea-8210-839bf07eded7" />
            /// <owner alias="ROrleth" />
            public void UpdateDefinitionFromLinkedStyle(XmlNode linkedNode, XmlNamespaceManager nsm)
            {
                if (!this.HasLinkedStyle)
                    return;

                this.readListLevelsFromAbsNode(linkedNode, nsm);
            }

            /// <id guid="0e05c34c-f257-4c76-8916-3059af84e333" />
            /// <owner alias="ROrleth" />
            private void readListLevelsFromAbsNode(XmlNode absNumNode, XmlNamespaceManager nsm)
            {
                XmlNodeList levelNodes = absNumNode.SelectNodes("./w:lvl", nsm);

                if (this.listLevels == null)
                {
                    this.listLevels = new Dictionary<string, ListLevel>(levelNodes.Count);
                }

                // loop through the levels it defines and instantiate those
                foreach (XmlNode levelNode in levelNodes)
                {
                    ListLevel level = new ListLevel(levelNode, nsm);

                    this.listLevels[level.ID] = level;
                }
            }

            private string linkedStyleId;

            /// <summary>
            /// returnts the ID of the linked style
            /// </summary>
            /// <id guid="ae2caeec-2d86-4e5f-b816-d508f6f2c893" />
            /// <owner alias="ROrleth" />
            public string LinkedStyleId
            {
                get
                {
                    return this.linkedStyleId;
                }
            }

            /// <summary>
            /// indicates whether there is a linked style
            /// </summary>
            /// <id guid="75d74788-9839-448e-ae23-02d40e013d98" />
            /// <owner alias="ROrleth" />
            public bool HasLinkedStyle
            {
                get
                {
                    return !String.IsNullOrEmpty(this.linkedStyleId);
                }
            }


            private string abstractNumDefId;

            /// <summary>
            /// returns the ID of this abstract number list definition
            /// </summary>
            public string ID
            {
                get
                {
                    return this.abstractNumDefId;
                }
            }

            public Dictionary<String, ListLevel> ListLevels
            {
                get
                {
                    return this.listLevels;
                }
            }

            public int LevelCount
            {
                get
                {
                    if (this.ListLevels != null)
                        return this.listLevels.Count;
                    else
                        return 0;
                }
            }
        }

        /// <summary>
        /// private helper class to deal with number lists
        /// </summary>
        private class ListNumberingDefinition
        {
            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="numNode"></param>
            /// <param name="nsm"></param>
            /// <param name="abstractListDefinitions"></param>
            public ListNumberingDefinition(XmlNode numNode, XmlNamespaceManager nsm, Dictionary<string, AbstractListNumberingDefinition> abstractListDefinitions)
            {
                this.listNumberId = getAttributeValue(numNode, "w:numId");

                XmlNode abstractNumNode = numNode.SelectSingleNode("./w:abstractNumId", nsm);

                if (abstractNumNode != null)
                {
                    this.abstractListDefinition = abstractListDefinitions[getAttributeValue(abstractNumNode, ValAttrName)];

                    // Create local overrides for the list number levels
                    overrideLevels = new Dictionary<string, ListLevel>();

                    // propagate the level overrides into the current list number level definition
                    XmlNodeList levelOverrideNodes = numNode.SelectNodes("./w:lvlOverride", nsm);

                    if (levelOverrideNodes != null)
                    {
                        foreach (XmlNode overrideNode in levelOverrideNodes)
                        {
                            string overrideLevelId = getAttributeValue(overrideNode, "w:ilvl");
                            XmlNode node = overrideNode.SelectSingleNode("./w:lvl", nsm);
                            if (node == null)
                                node = overrideNode;

                            if (!String.IsNullOrEmpty(overrideLevelId))
                            {
                                ListLevel newLevel = new ListLevel(this.abstractListDefinition.ListLevels[overrideLevelId]);
                                newLevel.SetOverrides(node, nsm);
                                overrideLevels.Add(overrideLevelId, newLevel);
                            }
                        }
                    }
                }
            }
            private AbstractListNumberingDefinition abstractListDefinition;
            private Dictionary<String, ListLevel> overrideLevels;

            /// <summary>
            /// increment the occurrence count of the specified level, reset the occurrence count of derived levels
            /// </summary>
            /// <param name="level"></param>
            public void IncrementCounter(string level)
            {
                FindLevel(level).IncrementCounter();

                // here's a bit where the decision to use strings as level IDs was bad - I need to loop through the derived levels and reset their counters
                UInt32 levelNumber = System.Convert.ToUInt32(level, CultureInfo.InvariantCulture) + 1;
                string levelString = levelNumber.ToString(CultureInfo.InvariantCulture);

                while (LevelExists(levelString))
                {
                    FindLevel(levelString).ResetCounter();
                    levelNumber++;
                    levelString = levelNumber.ToString(CultureInfo.InvariantCulture);
                }
            }

            private string listNumberId;

            /// <summary>
            /// numId of this list numbering schema
            /// </summary>
            public string ListNumberId
            {
                get
                {
                    return this.listNumberId;
                }
            }

            /// <summary>
            /// returns a string containing the current state of the counters, up to the indicated level
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            public string GetCurrentNumberString(string level)
            {
                string formatString = FindLevel(level).LevelText;
                StringBuilder result = new StringBuilder();
                string temp = string.Empty;

                for (int i = 0; i < formatString.Length; i++)
                {
                    temp = formatString.Substring(i, 1);

                    if (String.CompareOrdinal(temp, "%") == 0)
                    {
                        if (i < formatString.Length - 1)
                        {
                            string formatStringLevel = formatString.Substring(i + 1, 1);
                            // as it turns out, in the format string, the level is 1-based
                            UInt32 levelId = System.Convert.ToUInt32(formatStringLevel, CultureInfo.InvariantCulture) - 1;
                            result.Append(FindLevel(levelId.ToString(CultureInfo.InvariantCulture)).FormatValue());
                            i++;
                        }
                    }
                    else
                    {
                        result.Append(temp);
                    }
                }

                return result.ToString();
            }

            /// <summary>
            /// retrieve the font name that was specified for the list string
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            public string GetFont(string level)
            {
                return FindLevel(level).Font;
            }

            /// <summary>
            /// retrieve whether the level was a bullet list type
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            public bool IsBullet(string level)
            {
                return FindLevel(level).IsBullet;
            }

            /// <summary>
            /// returns whether the specific level ID exists - in testing we've seen some referential integrity issues due to Word bugs
            /// </summary>
            /// <param name="level">
            /// </param>
            /// <returns>
            /// </returns>
            /// <id guid="b94c13b8-7273-4f6a-927b-178d685fbe0f" />
            /// <owner alias="ROrleth" />
            public bool LevelExists(string level)
            {
                if (this.overrideLevels.ContainsKey(level))
                    return true;
                return this.abstractListDefinition.ListLevels.ContainsKey(level);
            }

            /// <summary>
            /// returns whether the specific level ID exists - in testing we've seen some referential integrity issues due to Word bugs
            /// </summary>
            public ListLevel FindLevel(string level)
            {
                if (this.overrideLevels.ContainsKey(level))
                    return this.overrideLevels[level];
                return this.abstractListDefinition.ListLevels[level];
            }
        }

        /// <summary>
        /// private helper class to deal with number definitions in styles
        /// </summary>
        private class StyleDefinition
        {
            private string m_Name;
            private string m_LevelId;
            private string m_NumId;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="styleNode"></param>
            /// <param name="nsm"></param>
            /// <param name="styles"></param>
            public StyleDefinition(XmlNode styleNode, XmlNamespaceManager nsm, Dictionary<string, StyleDefinition> styles)
            {
                m_Name = getAttributeValue(styleNode.ParentNode.ParentNode, "w:styleId");
                XmlNode child = styleNode.ParentNode.ParentNode.SelectSingleNode("./w:basedOn", nsm);
                m_LevelId = "0";
                m_NumId = null;
                if (child != null)
                {
                    string basedOnName = getAttributeValue(child, ValAttrName);
                    if (styles.ContainsKey(basedOnName))
                    {
                        StyleDefinition basedOn = styles[basedOnName];
                        if (basedOn != null)
                        {
                            m_LevelId = basedOn.m_LevelId;
                            m_NumId = basedOn.m_NumId;
                        }
                    }
                }
                child = styleNode.SelectSingleNode("./w:ilvl", nsm);
                if (child != null)
                    m_LevelId = getAttributeValue(child, ValAttrName);
                child = styleNode.SelectSingleNode("./w:numId", nsm);
                if (child != null)
                    m_NumId = getAttributeValue(child, ValAttrName);
            }

            public string Name
            {
                get { return m_Name; }
            }
            public void AddNumbering(XmlDocument mainDoc, XmlNode parent)
            {
                XmlNode numNode = mainDoc.CreateNode(XmlNodeType.Element, "w:numPr", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                XmlNode child = mainDoc.CreateNode(XmlNodeType.Element, "w:ilvl", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                XmlAttribute attr = mainDoc.CreateAttribute(ValAttrName, "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                attr.Value = m_LevelId;
                child.Attributes.Append(attr);
                numNode.AppendChild(child);
                child = mainDoc.CreateNode(XmlNodeType.Element, "w:numId", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                attr = mainDoc.CreateAttribute(ValAttrName, "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                attr.Value = m_NumId;
                child.Attributes.Append(attr);
                numNode.AppendChild(child);
                parent.AppendChild(numNode);
            }
        }

        /// <summary>
        /// count occurrences of numbered lists, save that as a hint on the numbered list node
        /// </summary>
        /// <param name="mainDoc"></param>
        /// <param name="nsm"></param>
        private static void HandleNumberedLists(XmlDocument mainDoc, XmlNamespaceManager nsm)
        {
            // count the number of different list numbering schemes
            XmlNodeList numberNodes = mainDoc.SelectNodes("/w:document/w:numbering/w:num", nsm);

            if (numberNodes.Count == 0)
            {
                return;
            }

            // initialize the abstract number list
            XmlNodeList abstractNumNodes = mainDoc.SelectNodes("/w:document/w:numbering/w:abstractNum", nsm);
            Dictionary<string, AbstractListNumberingDefinition> abstractListDefinitions = new Dictionary<string, AbstractListNumberingDefinition>(abstractNumNodes.Count);
            Dictionary<string, ListNumberingDefinition> instanceListDefinitions = new Dictionary<string, ListNumberingDefinition>(numberNodes.Count);

            // store the abstract list type definitions
            foreach (XmlNode abstractNumNode in abstractNumNodes)
            {
                AbstractListNumberingDefinition absNumDef = new AbstractListNumberingDefinition(abstractNumNode, nsm);

                abstractListDefinitions[absNumDef.ID] = absNumDef;
            }

            // now go through the abstract list definitions and update those that are linked to other styles
            foreach (KeyValuePair<string, AbstractListNumberingDefinition> absNumDef in abstractListDefinitions)
            {
                if (absNumDef.Value.HasLinkedStyle)
                {
                    // find the linked style
                    string linkStyleXPath = "/w:document/w:numbering/w:abstractNum/w:styleLink[@w:val=\"" + absNumDef.Value.LinkedStyleId + "\"]";
                    XmlNode linkedStyleNode = mainDoc.SelectSingleNode(linkStyleXPath, nsm);

                    if (linkedStyleNode != null)
                    {
                        absNumDef.Value.UpdateDefinitionFromLinkedStyle(linkedStyleNode.ParentNode, nsm);
                    }
                }
            }

            // instantiate the list number definitions
            foreach (XmlNode numNode in numberNodes)
            {
                ListNumberingDefinition listDef = new ListNumberingDefinition(numNode, nsm, abstractListDefinitions);

                instanceListDefinitions[listDef.ListNumberId] = listDef;
            }

            // Get styles with numbering definitions
            XmlNodeList stylesWithNumbers = mainDoc.SelectNodes("/w:document/w:styles/w:style/w:pPr/w:numPr", nsm);
            Dictionary<string, StyleDefinition> styleDefinitions = new Dictionary<string, StyleDefinition>(stylesWithNumbers.Count);
            foreach (XmlNode styleNode in stylesWithNumbers)
            {
                // Check to see if it is based on a style that is not in the definitions list yet?
                StyleDefinition styleDef = new StyleDefinition(styleNode, nsm, styleDefinitions);
                styleDefinitions[styleDef.Name] = styleDef;
            }

            XmlNodeList styleParagraphs = mainDoc.SelectNodes("//w:pPr/w:pStyle", nsm);
            foreach (XmlNode paragraph in styleParagraphs)
            {
                string styleName = getAttributeValue(paragraph, ValAttrName);
                if (!String.IsNullOrEmpty(styleName) && styleDefinitions.ContainsKey(styleName))
                {
                    XmlNode oldNode = paragraph.ParentNode.SelectSingleNode("./w:numPr", nsm);
                    if (oldNode == null)
                        styleDefinitions[styleName].AddNumbering(mainDoc, paragraph.ParentNode);
                }
            }

            XmlNodeList listNodes = mainDoc.SelectNodes("//w:numPr/w:ilvl", nsm);

            foreach (XmlNode node in listNodes)
            {
                string levelId = getAttributeValue(node, ValAttrName);
                XmlNode numIdNode = node.ParentNode.SelectSingleNode("./w:numId", nsm);

                if (!String.IsNullOrEmpty(levelId) && numIdNode != null)
                {
                    string numId = getAttributeValue(numIdNode, ValAttrName);

                    if (!String.IsNullOrEmpty(numId) && instanceListDefinitions.ContainsKey(numId) && instanceListDefinitions[numId].LevelExists(levelId))
                    {
                        XmlAttribute counterAttr = mainDoc.CreateAttribute("numString");

                        instanceListDefinitions[numId].IncrementCounter(levelId);
                        counterAttr.Value = instanceListDefinitions[numId].GetCurrentNumberString(levelId) + " ";

                        node.Attributes.Append(counterAttr);

                        string font = instanceListDefinitions[numId].GetFont(levelId);

                        if (!String.IsNullOrEmpty(font))
                        {
                            XmlAttribute fontAttr = mainDoc.CreateAttribute("numFont");

                            fontAttr.Value = font;

                            node.Attributes.Append(fontAttr);
                        }

                        if (instanceListDefinitions[numId].IsBullet(levelId))
                        {
                            XmlAttribute bulletAttr = mainDoc.CreateAttribute("isBullet");

                            bulletAttr.Value = "true";

                            node.Attributes.Append(bulletAttr);
                        }
                    }
                }
            }
        }
        internal static string getAttributeValue(XmlNode node, string name)
        {
            string value = string.Empty;

            XmlAttribute attribute = node.Attributes[name];
            if (attribute != null && attribute.Value != null)
            {
                value = attribute.Value;
            }

            return value;
        }
#endif
    }
}
