// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RichTextBoxExWriter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RichTextBoxExWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Implementation of <see cref="IExtendedWriter"/> that writes to a <see cref="RichTextBoxEx"/>.
    /// </summary>
    public class RichTextBoxExWriter : TextWriter, IExtendedWriter
    {
        private readonly RichTextBoxEx textBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxExWriter"/> class.
        /// </summary>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        public RichTextBoxExWriter(RichTextBoxEx textBox)
        {
            this.textBox = textBox;
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="Encoding"/> in which the output is written.
        /// </summary>
        /// <returns>
        /// The Encoding in which the output is written.
        /// </returns>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(char value)
        {
            this.textBox.AppendText(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the given range of characters to this writer.
        /// </summary>
        /// <param name="buffer">
        /// The array of characters.
        /// </param>
        /// <param name="index">
        /// The start index.
        /// </param>
        /// <param name="count">
        /// The number of characters.
        /// </param>
        public override void Write(char[] buffer, int index, int count)
        {
            this.textBox.AppendText(new string(buffer, index, count));
        }

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Write(string value)
        {
            this.textBox.AppendText(value);
        }

        /// <summary>
        /// Writes a link to this writer.
        /// </summary>
        /// <param name="text">
        /// The link text.
        /// </param>
        public virtual void WriteLink(string text)
        {
            this.textBox.InsertLink(text);
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
        public virtual void WriteLink(string text, string hyperlink)
        {
            this.textBox.InsertLink(text, hyperlink);
        }
    }
}