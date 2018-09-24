// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorFieldField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorFieldField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.ErrorField"/> field.
    /// </summary>
    public class ErrorFieldField : ByteFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFieldField"/> class.
        /// </summary>
        /// <param name="erroneousField">
        /// The type of the erroneous field.
        /// </param>
        public ErrorFieldField(FieldType erroneousField)
            : base(FieldType.ErrorField, (byte)erroneousField)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFieldField"/> class.
        /// </summary>
        internal ErrorFieldField()
            : base(FieldType.ErrorField)
        {
        }

        /// <summary>
        /// Gets the type of the erroneous field.
        /// </summary>
        public FieldType ErroneousField
        {
            get
            {
                return (FieldType)this.Value;
            }
        }
    }
}