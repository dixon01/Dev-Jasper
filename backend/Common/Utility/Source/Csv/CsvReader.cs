// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSVReader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A data-reader style interface for reading CSV files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Csv
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A data-reader style interface for reading CSV files.
    /// </summary>
    public class CsvReader : IDisposable
    {
        #region Private variables
        private readonly Stream stream;
        private readonly StreamReader reader;
        private char separator = ';';
        private int readlines;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// Create a new reader for the given stream.
        /// </summary>
        /// <param name="s">
        /// The stream to read the CSV from.
        /// </param>
        public CsvReader(Stream s)
            : this(s, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// Create a new reader for the given stream and encoding.
        /// </summary>
        /// <param name="s">
        /// The stream to read the CSV from.
        /// </param>
        /// <param name="enc">
        /// The encoding used.
        /// </param>
        public CsvReader(Stream s, Encoding enc)
        {
            this.stream = s;
            if (!s.CanRead)
            {
                throw new CsvReaderException("Could not read the given CSV stream!");
            }

            this.reader = (enc != null) ? new StreamReader(s, enc) : new StreamReader(s);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// Creates a new reader for the given text file path.
        /// </summary>
        /// <param name="filename">
        /// The name of the file to be read.
        /// </param>
        public CsvReader(string filename)
            : this(filename, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// Creates a new reader for the given text file path and encoding.
        /// </summary>
        /// <param name="filename">
        /// The name of the file to be read.
        /// </param>
        /// <param name="enc">
        /// The encoding used.
        /// </param>
        public CsvReader(string filename, Encoding enc)
            : this(File.OpenRead(filename), enc)
        {
        }

        ~CsvReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets Separator.
        /// </summary>
        public char Separator
        {
            get
            {
                return this.separator;
            }

            set
            {
                this.separator = value;
            }
        }

        /// <summary>
        /// Disposes the CSVReader. The underlying stream is closed.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the fields for the next row of CSV data (or null if at EOF)
        /// </summary>
        /// <returns>A string array of fields or null if at the end of file.</returns>
        public string[] GetCsvLine()
        {
            string data = this.reader.ReadLine();
            this.readlines++;
            if (data == null)
            {
                return null;
            }

            if (data.Length == 0)
            {
                return new string[0];
            }

            var result = new ArrayList();
            this.ParseCsvFields(result, data);
            return (string[])result.ToArray(typeof(string));
        }

        /// <summary>
        /// Gets the index of the current line.
        /// </summary>
        /// <returns>
        /// The index of the current line.
        /// </returns>
        public int CurrentLine()
        {
            return this.readlines;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Closing the reader closes the underlying stream, too
                if (this.reader != null)
                {
                    this.reader.Close();
                }
                else if (this.stream != null)
                {
                    this.stream.Close(); // In case we failed before the reader was constructed
                }
            }
        }

        // Parses the CSV fields and pushes the fields into the result arraylist
        private void ParseCsvFields(ArrayList result, string data)
        {
            int pos = -1;
            while (pos < data.Length)
            {
                result.Add(this.ParseCsvField(data, ref pos));
            }
        }

        // Parses the field at the given position of the data, modified pos to match
        // the first unparsed position and returns the parsed field
        private string ParseCsvField(string data, ref int startSeparatorPosition)
        {
            if (startSeparatorPosition == data.Length - 1)
            {
                startSeparatorPosition++;

                // The last field is empty
                return string.Empty;
            }

            int fromPos = startSeparatorPosition + 1;

            // Determine if this is a quoted field
            if (data[fromPos] == '"')
            {
                // If we're at the end of the string, let's consider this a field that
                // only contains the quote
                if (fromPos == data.Length - 1)
                {
                    return "\"";
                }

                // Otherwise, return a string of appropriate length with double quotes collapsed
                // Note that FSQ returns data.Length if no single quote was found
                int nextSingleQuote = this.FindSingleQuote(data, fromPos + 1);
                startSeparatorPosition = nextSingleQuote + 1;
                return data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1).Replace("\"\"", "\"");
            }

            // The field ends in the next comma or EOL
            int nextComma = data.IndexOf(this.separator, fromPos);
            if (nextComma == -1)
            {
                startSeparatorPosition = data.Length;
                return data.Substring(fromPos);
            }

            startSeparatorPosition = nextComma;
            return data.Substring(fromPos, nextComma - fromPos);
        }

        // Returns the index of the next single quote mark in the string
        // (starting from startFrom)
        private int FindSingleQuote(string data, int startFrom)
        {
            int i = startFrom - 1;
            while (++i < data.Length)
            {
                if (data[i] == '"')
                {
                    // If this is a double quote, bypass the chars
                    if (i < data.Length - 1 && data[i + 1] == '"')
                    {
                        i++;
                    }
                    else
                    {
                        return i;
                    }
                }
            }

            // If no quote found, return the end value of i (data.Length)
            return i;
        }
    }
}
