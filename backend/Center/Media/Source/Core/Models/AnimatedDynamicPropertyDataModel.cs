// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedDynamicPropertyDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The animation data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// The animation data model.
    /// </summary>
    [Serializable]
    public class AnimatedDynamicPropertyDataModel : DynamicPropertyDataModel
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicPropertyDataModel"/> class.
        /// </summary>
        public AnimatedDynamicPropertyDataModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicPropertyDataModel"/> class.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to be set.
        /// </param>
        public AnimatedDynamicPropertyDataModel(EvalDataModelBase evaluation)
            : base(evaluation)
        {
        }

        /// <summary>
        /// Gets or sets the animation to be used when this property changes.
        /// </summary>
        public PropertyChangeAnimationDataModel Animation { get; set; }

        /// <summary>
        /// Reads a <see cref="AnimatedDynamicPropertyDataModel"/> from the given XML reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A new <see cref="AnimatedDynamicPropertyDataModel"/> deserialized from the given reader.
        /// </returns>
        internal static new AnimatedDynamicPropertyDataModel ReadFromXml(XmlReader reader)
        {
            var property = new AnimatedDynamicPropertyDataModel();
            ((IXmlSerializable)property).ReadXml(reader);
            return property;
        }

        /// <summary>
        /// Reads all XML attributes when de-serializing.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected override void ReadXmlAttributes(XmlReader reader)
        {
            base.ReadXmlAttributes(reader);

            var typeAttr = reader.GetAttribute("AnimationType");
            var durationAttr = reader.GetAttribute("AnimationDuration");
            if (string.IsNullOrEmpty(typeAttr) || string.IsNullOrEmpty(durationAttr))
            {
                return;
            }

            var animation = new PropertyChangeAnimationDataModel();
            try
            {
                animation.Type =
                    (PropertyChangeAnimationType)Enum.Parse(typeof(PropertyChangeAnimationType), typeAttr, true);
                animation.Duration = XmlConvert.ToTimeSpan(durationAttr);
            }
            catch
            {
                return;
            }

            this.Animation = animation;
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

            if (this.Animation == null || this.Animation.Type == PropertyChangeAnimationType.None)
            {
                return;
            }

            writer.WriteAttributeString("AnimationType", this.Animation.Type.ToString());
            writer.WriteAttributeString("AnimationDuration", XmlConvert.ToString(this.Animation.Duration));
        }
    }
}
