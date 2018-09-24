// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorMessageField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.ErrorMessage"/> field.
    /// </summary>
    public class ErrorMessageField : StringFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageField"/> class.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message string (max 255 characters).
        /// </param>
        public ErrorMessageField(string errorMessage)
            : base(FieldType.ErrorMessage, errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageField"/> class.
        /// </summary>
        internal ErrorMessageField()
            : base(FieldType.ErrorMessage)
        {
        }
    }
}