// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedDynamicProperty.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnimatedDynamicProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Common
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Dynamic property that is animated when it changes.
    /// </summary>
    [Serializable]
    public class AnimatedDynamicProperty : DynamicProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicProperty"/> class.
        /// </summary>
        public AnimatedDynamicProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicProperty"/> class.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to be set for <see cref="DynamicProperty.Evaluation"/>.
        /// </param>
        public AnimatedDynamicProperty(EvalBase evaluation)
            : base(evaluation)
        {
        }

        /// <summary>
        /// Gets or sets the animation to be used when this property changes.
        /// </summary>
        public PropertyChangeAnimation Animation { get; set; }

        /// <summary>
        /// Reads an <see cref="AnimatedDynamicProperty"/> from the given XML reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A new <see cref="AnimatedDynamicProperty"/> deserialized from the given reader.
        /// </returns>
        internal static new AnimatedDynamicProperty ReadFromXml(XmlReader reader)
        {
            var property = new AnimatedDynamicProperty();
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

            var animation = new PropertyChangeAnimation();
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