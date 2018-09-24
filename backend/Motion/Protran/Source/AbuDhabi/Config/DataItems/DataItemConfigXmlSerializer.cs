// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemConfigXmlSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializer class for DataItemConfig objects.
    /// </summary>
    internal class DataItemConfigXmlSerializer
    {
        /// <summary>
        /// Deserializes a <see cref="DataItemConfig"/> from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>A config object or null if the deserialized object didn't implement TelegramConfig.</returns>
        public DataItemConfig Deserialize(XmlReader reader)
        {
            var name = reader.Name;
            var typeName = string.Format("{0}.{1}", this.GetType().Namespace, name.Replace("-", "Minus"));
            var type = Type.GetType(typeName, false, true) ?? typeof(DataItemConfig);

            var ser = new XmlSerializer(type);
            var regex = new Regex(string.Format(@"(</?){0}(\W)", name), RegexOptions.IgnoreCase);
            var outerXml = reader.ReadOuterXml();
            outerXml = regex.Replace(outerXml, string.Format("$1{0}$2", type.Name));

            var dataItem = ser.Deserialize(new StringReader(outerXml)) as DataItemConfig;
            if (dataItem != null)
            {
                dataItem.Name = name;
            }

            return dataItem;
        }
    }
}
