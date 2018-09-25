// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitNameField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitNameField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.UnitName"/> field.
    /// </summary>
    public class UnitNameField : StringFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitNameField"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name (max 255 characters).
        /// </param>
        public UnitNameField(string unitName)
            : base(FieldType.UnitName, unitName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitNameField"/> class.
        /// </summary>
        internal UnitNameField()
            : base(FieldType.UnitName)
        {
        }
    }
}