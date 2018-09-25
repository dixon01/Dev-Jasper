// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Integer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Integer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Transformation config for string to integer transformation.
    /// </summary>
    [Serializable]
    public class Integer : TransformationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Integer"/> class.
        /// </summary>
        public Integer()
        {
            this.NumberStyle = NumberStyles.Integer;
        }

        /// <summary>
        /// Gets or sets the NumberStyle to be used in <see cref="int.Parse(string,NumberStyles)"/>.
        /// </summary>
        [DefaultValue(NumberStyles.Integer)]
        public NumberStyles NumberStyle { get; set; }
    }
}
