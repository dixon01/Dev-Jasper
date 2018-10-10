// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ParseException.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using System;

    /// <summary>
    /// the parse exception
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        public ParseException(string message)
            : base(message)
        {
        }
    }
}