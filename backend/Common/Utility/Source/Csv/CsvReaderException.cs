// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvReaderException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Exception class for CSVReader exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Csv
{
    using System;

    /// <summary>
    /// Exception class for CSVReader exceptions.
    /// </summary>
    public class CsvReaderException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderException"/> class. 
        /// Constructs a new exception object with the given message.
        /// </summary>
        /// <param name="message">
        /// The exception message.
        /// </param>
        public CsvReaderException(string message)
            : base(message)
        {
        }
    }
}