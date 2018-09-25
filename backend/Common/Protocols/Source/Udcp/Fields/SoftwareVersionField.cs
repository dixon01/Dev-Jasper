// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareVersionField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwareVersionField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.SoftwareVersion"/> field.
    /// </summary>
    public class SoftwareVersionField : StringFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareVersionField"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name (max 255 characters).
        /// </param>
        public SoftwareVersionField(string unitName)
            : base(FieldType.SoftwareVersion, unitName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareVersionField"/> class.
        /// </summary>
        internal SoftwareVersionField()
            : base(FieldType.SoftwareVersion)
        {
        }
    }
}