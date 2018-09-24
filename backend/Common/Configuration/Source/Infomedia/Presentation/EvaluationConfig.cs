// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation
{
    using System.Xml;

    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Evaluation config that allows to reuse the same evaluation multiple
    /// times using a <see cref="EvaluationEval"/>.
    /// </summary>
    public partial class EvaluationConfig
    {
        /// <summary>
        /// Reads all XML attributes when de-serializing.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);
            this.Name = reader.GetAttribute("Name");
        }

        /// <summary>
        /// Writes all XML attributes when serializing.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteAttributeString("Name", this.Name);
        }
    }
}