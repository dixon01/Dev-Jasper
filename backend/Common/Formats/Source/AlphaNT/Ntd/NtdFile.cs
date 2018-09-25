// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NtdFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NtdFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;
    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    /// <summary>
    /// Class to read Alpha NT NTD files.
    /// </summary>
    public class NtdFile : IDisposable
    {
        private static readonly Encoding DefaultEncoding = Encoding.GetEncoding(1252);

        /*
         * General file format:
         * ====================
         * Header (200 bytes)
         * Fonts (max 10 fonts)
         * Addresses of font files (30 or 48 bytes)
         * Signs 1...15
         * BLD file (if available)
         * Footer
         */
        private readonly BinaryFileReader reader;

        private readonly List<HeaderSignOffsets> allHeaderSignOffets = new List<HeaderSignOffsets>();
        private readonly List<IntPtr> fontOffsets = new List<IntPtr>();

        // ReSharper disable NotAccessedField.Local
        private int signCount;

        private bool hasBld;

        private IntPtr fontAddressesOffset;

        private IntPtr fileNameOffset;

        private IntPtr schedule2DateOffset;

        // ReSharper restore NotAccessedField.Local

        /// <summary>
        /// Initializes a new instance of the <see cref="NtdFile"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename of the NTD file.
        /// </param>
        public NtdFile(string filename)
            : this(File.OpenRead(filename), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NtdFile"/> class.
        /// </summary>
        /// <param name="input">
        /// The underlying input stream.
        /// </param>
        /// <param name="closeOnDispose">
        /// A flag indicating whether the <see cref="input"/> should be closed and disposed when this
        /// object is disposed (see <see cref="Dispose"/>).
        /// </param>
        public NtdFile(Stream input, bool closeOnDispose)
        {
            this.reader = new BinaryFileReader(input, closeOnDispose);

            this.ReadHeader();
            this.ReadFontOffsets();
            this.ReadFooter();

            // fill the sign information
            var signs = new List<ISignInfo>();
            for (int i = 0; i < 15; i++)
            {
                var offsets = this.allHeaderSignOffets[i];

                // filter out all undefined ("zero") signs
                if (offsets.OffsetSignSize != IntPtr.Zero)
                {
                    signs.Add(new SignInfo(i + 1, this.reader, offsets));

                    this.CheckHasColorTelegrams(offsets.OffsetTextsL);
                    this.CheckHasColorTelegrams(offsets.OffsetTextsZ);
                }
            }

            this.Signs = signs.ToArray();
        }

        /// <summary>
        /// Gets the list of signs defined in this file.
        /// </summary>
        public ISignInfo[] Signs { get; private set; }

        /// <summary>
        /// Gets the footer string.
        /// </summary>
        public string Footer { get; private set; }

        /// <summary>
        /// Gets the font with the given index.
        /// </summary>
        /// <param name="fontIndex">
        /// The font index (0 ... 9 or 15 [extended format]).
        /// This index usually comes from <see cref="TextPrimitive.FontIndex"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IFont"/>.
        /// </returns>
        public IFont GetFont(int fontIndex)
        {
            if (fontIndex < 0 || fontIndex >= this.fontOffsets.Count)
            {
                throw new ArgumentOutOfRangeException("fontIndex");
            }

            var offset = this.fontOffsets[fontIndex];
            if (offset == IntPtr.Zero)
            {
                return null;
            }

            this.reader.SetPosition(offset);
            return new FontFile(this.reader, false);
        }

        /// <summary>
        /// Gets the bitmap at the given address.
        /// </summary>
        /// <param name="bitmapAddress">
        /// The bitmap address.
        /// This address usually comes from <see cref="BitmapPrimitive.BitmapAddress"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IBitmap"/>.
        /// </returns>
        public IBitmap GetBitmap(IntPtr bitmapAddress)
        {
            this.reader.VerifyAdress(bitmapAddress);
            this.reader.SetPosition(bitmapAddress);

            if (!this.reader.ExtendedFormat)
            {
                return new EgrBitmap(this.reader, false);
            }

            var first = this.reader.ReadLsbUInt16();
            this.reader.SetPosition(bitmapAddress);

            // first bit tells us if it is color or not
            if ((first & 0x8000) == 0)
            {
                return new EglBitmap(this.reader, false);
            }

            return new EgfBitmap(this.reader, false);
        }

        /// <summary>
        /// Gets the string at the given address.
        /// </summary>
        /// <param name="textAddress">
        /// The address of the string in the file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetString(IntPtr textAddress)
        {
            this.reader.VerifyAdress(textAddress);
            this.reader.SetPosition(textAddress);
            var length = this.reader.ReadByte();

            // TODO: support unicode???
            return DefaultEncoding.GetString(this.reader.ReadBytes(length).ToArray(), 0, length);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.reader.Close();
        }

        private void ReadHeader()
        {
            this.reader.SetPosition(IntPtr.Zero);
            this.reader.ExpectIdentifier('R');

            this.signCount = this.reader.ReadByte();
            if (this.signCount > 15)
            {
                throw new FileFormatException("Expected 0..15 signs but found " + this.signCount);
            }

            this.hasBld = this.reader.ReadByte() == 'B';
            this.reader.ExtendedFormat = this.reader.ReadByte() == '4';

            this.reader.ExpectIdentifier('C');
            this.fontAddressesOffset = this.reader.ReadLsbUInt24Address();

            for (int i = 1; i <= 15; i++)
            {
                var info = new HeaderSignOffsets();
                this.allHeaderSignOffets.Add(info);
                this.reader.ExpectIdentifier('L');
                info.OffsetTextsL = this.reader.ReadLsbUInt24Address();
                this.reader.ExpectIdentifier('Z');
                info.OffsetTextsZ = this.reader.ReadLsbUInt24Address();
                this.reader.ExpectIdentifier('I');
                info.OffsetSignSize = this.reader.ReadLsbUInt24Address();
            }

            this.reader.ExpectIdentifier('A');
            var fileSize = this.reader.ReadLsbUInt24();
            if (fileSize > this.reader.Length)
            {
                throw new FileFormatException(
                    string.Format(
                        "File size is incorrect: expected {0} but found {1} in header", this.reader.Length, fileSize));
            }

            this.reader.ExpectIdentifier('F');
            this.fileNameOffset = this.reader.ReadLsbUInt24Address();

            var identifierD = this.reader.ReadByte();
            if (identifierD != 0)
            {
                if (identifierD != 'D')
                {
                    throw new FileFormatException(
                        string.Format("Expected identifier 'D' or 0 but got '{0}'", identifierD));
                }

                this.schedule2DateOffset = this.reader.ReadLsbUInt24Address();
            }
            else
            {
                // ignore 3 bytes
                this.reader.ReadLsbUInt24();
            }

            if (this.reader.ReadByte() != 0xFF || this.reader.ReadByte() != 0xFF)
            {
                throw new FileFormatException("Header doesn't end with FFFF");
            }
        }

        private void ReadFontOffsets()
        {
            this.reader.SetPosition(this.fontAddressesOffset);

            var fontCount = this.reader.ExtendedFormat ? 16 : 10;
            for (int i = 0; i < fontCount; i++)
            {
                this.fontOffsets.Add(this.reader.ReadLsbUInt24Address());
            }
        }

        private void ReadFooter()
        {
            this.Footer = this.GetString(this.fileNameOffset);
            this.reader.HasColors = Regex.IsMatch(this.Footer, @";V\d+.\d+.\d+F");

            var match = Regex.Match(this.Footer, @";Prog:([^;]+)");
            if (!match.Success)
            {
                // we assume "standard" when there is no "Prog"
                return;
            }

            var prog = match.Groups[1].Value;
            if (prog == "vierByteGraphik")
            {
                this.reader.ExtendedFormat = true;
            }
            else if (prog != "standard")
            {
                throw new NotSupportedException("Program version currently not supported: " + match.Groups[1].Value);
            }
        }

        private void CheckHasColorTelegrams(IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return;
            }

            // check if the sign is color mode (that means we are in color mode)
            // usually we know this from the footer, but older versions don't have this
            this.reader.SetPosition(new IntPtr(address.ToInt32() - 1));
            if ((this.reader.ReadByte() & 0x80) != 0)
            {
                this.reader.HasColors = true;
            }
        }

        private class HeaderSignOffsets
        {
            public IntPtr OffsetTextsL { get; set; }

            public IntPtr OffsetTextsZ { get; set; }

            public IntPtr OffsetSignSize { get; set; }
        }

        private class SignInfo : ISignInfo
        {
            private readonly BinaryFileReader reader;

            private readonly HeaderSignOffsets headerSignOffsets;

            public SignInfo(int address, BinaryFileReader reader, HeaderSignOffsets headerSignOffsets)
            {
                this.Address = address;
                this.reader = reader;
                this.headerSignOffsets = headerSignOffsets;

                this.ReadDimensions();
            }

            public int Address { get; private set; }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public TelegramTypes TelegramTypes { get; private set; }

            public IEnumerable<ITelegramInfo> GetLineTelegrams()
            {
                return this.GetTelegrams(this.headerSignOffsets.OffsetTextsL);
            }

            public IEnumerable<ITelegramInfo> GetDestinationTelegrams()
            {
                return this.GetTelegrams(this.headerSignOffsets.OffsetTextsZ);
            }

            private void ReadDimensions()
            {
                this.reader.SetPosition(this.headerSignOffsets.OffsetSignSize);
                this.Width = this.ReadDimensionValue();
                this.Height = this.ReadDimensionValue();
                this.TelegramTypes = (TelegramTypes)this.reader.ReadByte();
            }

            private int ReadDimensionValue()
            {
                return this.reader.ExtendedFormat ? this.reader.ReadLsbUInt16() : this.reader.ReadByte();
            }

            private IEnumerable<ITelegramInfo> GetTelegrams(IntPtr offset)
            {
                if (offset == IntPtr.Zero)
                {
                    return new ITelegramInfo[0];
                }

                this.reader.SetPosition(offset);
                var telegrams = new List<ITelegramInfo>();

                ITelegramInfo telegram;
                while ((telegram = this.ReadTelegramInfo()) != null)
                {
                    telegrams.Add(telegram);
                }

                return telegrams;
            }

            private ITelegramInfo ReadTelegramInfo()
            {
                var mode = this.reader.ReadByte();
                if (mode == 0xFF)
                {
                    return null;
                }

                // chapters not mentioned below are currently not supported (non-standard format)
                if (!this.reader.ExtendedFormat)
                {
                    // chapter 3.4.1
                    return new SimpleTelegramInfo(mode, this.reader);
                }

                if (!this.reader.HasColors)
                {
                    // chapter 3.4.2
                    return new ExtendedTelegramInfo(mode, this.reader);
                }

                // chapter 3.4.4
                return new ColorTelegramInfo(mode, this.reader);
            }
        }
    }
}
