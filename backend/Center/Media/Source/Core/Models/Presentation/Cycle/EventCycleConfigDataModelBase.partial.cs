// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleConfigDataModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The event cycle config data model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation.Cycle
{
    using System.Xml;

    /// <summary>
    /// The event cycle config data model base.
    /// </summary>
    public partial class EventCycleConfigDataModelBase
    {
        /// <summary>
        /// Reads the given element when de-serializing.
        /// </summary>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// True if the element has been handled.
        /// </returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            if (elementName != "Trigger")
            {
                return base.ReadXmlElement(elementName, reader);
            }

            this.Trigger = GenericTriggerConfigDataModel.ReadFromXml(reader);
            return true;
        }

        /// <summary>
        /// Writes all XML elements when serializing.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteXmlElements(XmlWriter writer)
        {
            base.WriteXmlElements(writer);

            GenericTriggerConfigDataModel.WriteToXml("Trigger", this.Trigger, writer);
        }
    }
}
