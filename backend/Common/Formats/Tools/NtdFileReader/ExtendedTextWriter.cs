// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedTextWriter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtendedTextWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// <see cref="StreamWriter"/> that implements <see cref="IExtendedWriter"/>.
    /// </summary>
    internal class ExtendedTextWriter : StreamWriter, IExtendedWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTextWriter"/> class.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        public ExtendedTextWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }

        /// <summary>
        /// Writes a link to this writer.
        /// </summary>
        /// <param name="text">
        /// The link text.
        /// </param>
        public void WriteLink(string text)
        {
            this.Write(text);
        }

        /// <summary>
        /// Writes a link to this writer.
        /// </summary>
        /// <param name="text">
        /// The link text shown to the user.
        /// </param>
        /// <param name="hyperlink">
        /// The hyperlink content hidden from the user.
        /// </param>
        public void WriteLink(string text, string hyperlink)
        {
            this.Write(text);
        }
    }
}