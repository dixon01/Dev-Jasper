// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCycleConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Cycle
{
    using System.Xml;

    /// <summary>
    /// Base class for all event cycle configurations.
    /// </summary>
    public abstract partial class EventCycleConfigBase
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

            this.Trigger = GenericTriggerConfig.ReadFromXml(reader);
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

            GenericTriggerConfig.WriteToXml("Trigger", this.Trigger, writer);
        }
    }
}