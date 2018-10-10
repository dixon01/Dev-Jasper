// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationConfigDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The pool config data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation
{
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The pool config data model.
    /// </summary>
    public partial class EvaluationConfigDataModel
    {
        /// <summary>
        /// Gets or sets the reference count.
        /// </summary>
        [XmlAttribute("ReferenceCount")]
        public int ReferenceCount { get; set; }

        /// <summary>
        /// The read xml attributes.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            var attrName = reader.GetAttribute("Name");
            if (attrName != null)
            {
                this.Name = attrName;
            }

            var attrDisplayText = reader.GetAttribute("DisplayText");
            if (attrDisplayText != null)
            {
                this.DisplayText = attrDisplayText;
            }

            var attrReferenceCount = reader.GetAttribute("ReferenceCount");
            if (attrReferenceCount != null)
            {
                int referenceCount;
                int.TryParse(attrReferenceCount, out referenceCount);
                this.ReferenceCount = referenceCount;
            }
        }

        /// <summary>
        /// The read xml element.
        /// </summary>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        /// <summary>
        /// The write xml attributes.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("DisplayText", this.DisplayText);
            writer.WriteAttributeString("ReferenceCount", this.ReferenceCount.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The write xml elements.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteXmlElements(XmlWriter writer)
        {
        }
    }
}
