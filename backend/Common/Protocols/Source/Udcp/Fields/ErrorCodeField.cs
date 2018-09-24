// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCodeField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorCodeField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.ErrorCode"/> field.
    /// </summary>
    public class ErrorCodeField : ByteFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeField"/> class.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        public ErrorCodeField(ErrorCode errorCode)
            : base(FieldType.ErrorCode, (byte)errorCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeField"/> class.
        /// </summary>
        internal ErrorCodeField()
            : base(FieldType.ErrorCode)
        {
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public ErrorCode ErrorCode
        {
            get
            {
                return (ErrorCode)this.Value;
            }
        }
    }
}