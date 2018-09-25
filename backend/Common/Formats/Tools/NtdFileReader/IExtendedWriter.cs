// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendedWriter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IExtendedWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System;

    /// <summary>
    /// Text writer that allows to add links.
    /// </summary>
    public interface IExtendedWriter : IDisposable
    {
        // ReSharper disable MethodOverloadWithOptionalParameter

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(char value);

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
        void Write(char[] buffer, int index, int count);

        /// <summary>
        /// Closes this writer.
        /// </summary>
        void Close();

        /// <summary>
        /// Flushes this writer.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes the given array of characters to this writer.
        /// </summary>
        /// <param name="buffer">
        /// The array of characters.
        /// </param>
        void Write(char[] buffer);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(bool value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(int value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(uint value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(long value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(ulong value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(float value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(double value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(decimal value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(string value);

        /// <summary>
        /// Writes the given value to this writer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void Write(object value);

        /// <summary>
        /// Writes the formatted value to this writer.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        void Write(string format, object arg0);

        /// <summary>
        /// Writes the formatted value to this writer.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        /// <param name="arg1">
        /// The second argument.
        /// </param>
        void Write(string format, object arg0, object arg1);

        /// <summary>
        /// Writes the formatted value to this writer.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        /// <param name="arg1">
        /// The second argument.
        /// </param>
        /// <param name="arg2">
        /// The third argument.
        /// </param>
        void Write(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Writes the formatted value to this writer.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg">
        /// The arguments.
        /// </param>
        void Write(string format, params object[] arg);

        /// <summary>
        /// Writes a new-line to this writer.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(char value);

        /// <summary>
        /// Writes the given array of characters to this writer followed by a new-line.
        /// </summary>
        /// <param name="buffer">
        /// The array of characters.
        /// </param>
        void WriteLine(char[] buffer);

        /// <summary>
        /// Writes the given range of characters to this writer followed by a new-line.
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
        void WriteLine(char[] buffer, int index, int count);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(bool value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(int value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(uint value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(long value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(ulong value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(float value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(double value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(decimal value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(string value);

        /// <summary>
        /// Writes the given value to this writer followed by a new-line.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void WriteLine(object value);

        /// <summary>
        /// Writes the formatted value to this writer followed by a new-line.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        void WriteLine(string format, object arg0);

        /// <summary>
        /// Writes the formatted value to this writer followed by a new-line.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        /// <param name="arg1">
        /// The second argument.
        /// </param>
        void WriteLine(string format, object arg0, object arg1);

        /// <summary>
        /// Writes the formatted value to this writer followed by a new-line.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The first argument.
        /// </param>
        /// <param name="arg1">
        /// The second argument.
        /// </param>
        /// <param name="arg2">
        /// The third argument.
        /// </param>
        void WriteLine(string format, object arg0, object arg1, object arg2);

        /// <summary>
        /// Writes the formatted value to this writer followed by a new-line.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg">
        /// The arguments.
        /// </param>
        void WriteLine(string format, params object[] arg);

        /// <summary>
        /// Writes a link to this writer.
        /// </summary>
        /// <param name="text">
        /// The link text.
        /// </param>
        void WriteLink(string text);

        /// <summary>
        /// Writes a link to this writer.
        /// </summary>
        /// <param name="text">
        /// The link text shown to the user.
        /// </param>
        /// <param name="hyperlink">
        /// The hyperlink content hidden from the user.
        /// </param>
        void WriteLink(string text, string hyperlink);
    }
}