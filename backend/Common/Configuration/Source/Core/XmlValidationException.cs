// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlValidationException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Schema;

    /// <summary>
    /// Exception for cumulated <see cref="XmlSchemaException"/>s.
    /// </summary>
    public class XmlValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public XmlValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidationException"/> class.
        /// </summary>
        /// <param name="exceptions">
        /// The exceptions.
        /// </param>
        public XmlValidationException(List<XmlSchemaException> exceptions)
        {
            this.Exceptions = exceptions;
        }

        /// <summary>
        /// Gets the list of exceptions occurred during xml schema validation.
        /// </summary>
        public List<XmlSchemaException> Exceptions { get; private set; }
    }
}
