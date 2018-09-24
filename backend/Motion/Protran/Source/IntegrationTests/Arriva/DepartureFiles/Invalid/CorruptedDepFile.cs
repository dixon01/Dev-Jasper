// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorruptedDepFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles.Invalid
{
    /// <summary>
    /// Object tasked to represent a corrupted XML departures file.
    /// </summary>
    public class CorruptedDepFile : InvalidDepartureFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptedDepFile"/> class.
        /// </summary>
        public CorruptedDepFile()
        {
            this.Content =
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
    <departures xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    expiration=""2012-0""
    – as of here the file is truncated and therefore corrupt.";
        }
    }
}
