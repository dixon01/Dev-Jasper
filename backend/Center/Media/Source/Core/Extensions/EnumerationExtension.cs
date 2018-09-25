// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationExtension.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EnumerationExtension.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Markup;

    /// <summary>
    /// The EnumerationExtension.
    /// </summary>
    public class EnumerationExtension : MarkupExtension
    {
        private Type enumType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerationExtension"/> class.
        /// </summary>
        /// <param name="enumType">the enum type</param>
        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            this.EnumType = enumType;
        }

        /// <summary>
        /// Gets the enum type
        /// </summary>
        public Type EnumType
        {
            get
            {
                return this.enumType;
            }

            private set
            {
                if (this.enumType == value)
                {
                    return;
                }

                var type = Nullable.GetUnderlyingType(value) ?? value;

                if (type.IsEnum == false)
                {
                    throw new ArgumentException("Type must be an Enum.");
                }

                this.enumType = value;
            }
        }

        /// <summary>
        /// Gets or sets the localization prefix
        /// </summary>
        public string LocalizationPrefix { get; set; }

        /// <summary>
        /// Provides a value for the enumeration
        /// </summary>
        /// <param name="serviceProvider">the service provider</param>
        /// <returns>the value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(this.EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember
              {
                  Value = enumValue,
                  Description = this.GetDescription(enumValue)
              }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = this.EnumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;

            var result  = descriptionAttribute != null
              ? descriptionAttribute.Description
              : enumValue.ToString();

            var rm = Resources.MediaStrings.ResourceManager;
            result = rm.GetString(this.LocalizationPrefix + enumValue.ToString()) ?? enumValue.ToString();

            return result;
        }

        /// <summary>
        /// The Enumeration Member
        /// </summary>
        public class EnumerationMember
        {
            /// <summary>
            /// Gets or sets the description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the value
            /// </summary>
            public object Value { get; set; }
        }
    }
}