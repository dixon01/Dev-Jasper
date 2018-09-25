// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelConverterAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelConverterAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;

    /// <summary>
    /// Helps the conversion process.
    /// </summary>
    public sealed class DataViewModelConverterAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelConverterAttribute"/> class.
        /// </summary>
        /// <param name="converterType">Type of the converter.</param>
        /// <param name="conversionType">Type of the conversion.</param>
        public DataViewModelConverterAttribute(Type converterType, Type conversionType)
        {
            this.ConversionType = conversionType;
            this.ConverterType = converterType;
        }

        /// <summary>
        /// Gets the type of the conversion.
        /// </summary>
        /// <value>
        /// The type of the conversion.
        /// </value>
        public Type ConversionType { get; private set; }

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        /// <value>
        /// The type of the converter.
        /// </value>
        public Type ConverterType { get; private set; }
    }
}