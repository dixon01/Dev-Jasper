// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NtdInfoProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NtdInfoProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gorba.Common.Formats.AlphaNT.Ntd;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;
    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    /// <summary>
    /// The provider for information about an NTD file.
    /// </summary>
    public class NtdInfoProvider
    {
        private static readonly string Indent = new string(' ', 4);

        private readonly NtdFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="NtdInfoProvider"/> class.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public NtdInfoProvider(NtdFile file)
        {
            this.file = file;
        }

        /// <summary>
        /// Writes all information to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void WriteTo(IExtendedWriter writer)
        {
            this.WriteFooter(writer);
            this.WriteFonts(writer);
            this.WriteSigns(writer);
        }

        private void WriteSigns(IExtendedWriter writer)
        {
            writer.WriteLine("Signs:");
            foreach (var sign in this.file.Signs)
            {
                var dlz = new StringBuilder();
                if ((sign.TelegramTypes & TelegramTypes.Text) != 0)
                {
                    dlz.Append('D');
                }

                if ((sign.TelegramTypes & TelegramTypes.Line) != 0)
                {
                    dlz.Append('L');
                }

                if ((sign.TelegramTypes & TelegramTypes.Destination) != 0)
                {
                    dlz.Append('Z');
                }

                writer.Write(Indent);
                writer.WriteLine("Sign {0}: {1} x {2} (H x W) ({3})", sign.Address, sign.Height, sign.Width, dlz);
            }

            writer.WriteLine();
            foreach (var sign in this.file.Signs)
            {
                writer.WriteLine("Sign {0}:", sign.Address);
                writer.Write(Indent);
                writer.WriteLine("Size: {0} x {1} (H x W)", sign.Height, sign.Width);
                this.WriteTelegrams("L", sign.GetLineTelegrams(), writer);
                this.WriteTelegrams("Z", sign.GetDestinationTelegrams(), writer);
                writer.WriteLine();
            }
        }

        private void WriteTelegrams(string name, IEnumerable<ITelegramInfo> telegrams, IExtendedWriter writer)
        {
            var telegramList = telegrams.ToArray();
            writer.Write(Indent);
            writer.WriteLine("{0} Telegrams: ({1})", name, telegramList.Length);
            foreach (var telegram in telegramList)
            {
                writer.Write(Indent);
                writer.Write(Indent);
                writer.WriteLine(
                    "{0}{1:000} for [{2}] [{3}]: ({4})",
                    name.ToLower(),
                    telegram.TelegramNumber,
                    telegram.Schedule,
                    telegram.ScheduleMode,
                    telegram.PrimitiveCount);
                foreach (var primitive in telegram.GetPrimitives())
                {
                    this.WritePrimitive(primitive, writer);
                }
            }
        }

        private void WritePrimitive(GraphicPrimitiveBase primitive, IExtendedWriter writer)
        {
            var bitmap = primitive as BitmapPrimitive;
            if (bitmap != null)
            {
                this.WriteBitmap(bitmap, writer);
                return;
            }

            var text = primitive as TextPrimitive;
            if (text != null)
            {
                this.WriteText(text, writer);
                return;
            }

            var delete = primitive as DeleteAreaPrimitive;
            if (delete != null)
            {
                this.WriteDeleteArea(delete, writer);
                return;
            }

            var invert = primitive as InvertAreaPrimitive;
            if (invert != null)
            {
                this.WriteInvertArea(invert, writer);
                return;
            }

            writer.Write(Indent);
            writer.Write(Indent);
            writer.Write(Indent);
            writer.WriteLine("{0} ???", primitive.GetType().Name);
        }

        private void WriteBitmap(BitmapPrimitive bitmap, IExtendedWriter writer)
        {
            writer.Write(Indent);
            writer.Write(Indent);
            writer.Write(Indent);
            writer.WriteLink("Bitmap", "bmp:" + bitmap.BitmapAddress.ToInt32());
            writer.WriteLine(" @[{0},{1}]", bitmap.OffsetX, bitmap.OffsetY);
        }

        private void WriteText(TextPrimitive text, IExtendedWriter writer)
        {
            writer.Write(Indent);
            writer.Write(Indent);
            writer.Write(Indent);

            var value = this.file.GetString(text.TextAddress);
            var font = this.file.GetFont(text.FontIndex);

            writer.Write("\"{0}\" @[{1},{2}] Font: ", value, text.OffsetX, text.OffsetY);
            if (font == null)
            {
                writer.Write("<unknown>");
            }
            else
            {
                writer.WriteLink(font.Name.Trim(), "fnt:" + text.FontIndex);
            }

            writer.WriteLine();
        }

        private void WriteDeleteArea(DeleteAreaPrimitive delete, IExtendedWriter writer)
        {
            writer.Write(Indent);
            writer.Write(Indent);
            writer.Write(Indent);
            writer.WriteLine("Delete [{0},{1},{2},{3}]", delete.OffsetX, delete.OffsetY, delete.Width, delete.Height);
        }

        private void WriteInvertArea(InvertAreaPrimitive invert, IExtendedWriter writer)
        {
            writer.Write(Indent);
            writer.Write(Indent);
            writer.Write(Indent);
            writer.WriteLine("Invert [{0},{1},{2},{3}]", invert.OffsetX, invert.OffsetY, invert.Width, invert.Height);
        }

        private void WriteFonts(IExtendedWriter writer)
        {
            writer.WriteLine("Fonts:");
            for (int fontIndex = 0; fontIndex < 16; fontIndex++)
            {
                try
                {
                    var font = this.file.GetFont(fontIndex);

                    if (font == null)
                    {
                        continue;
                    }

                    writer.Write(Indent);
                    writer.Write("{0}: ", fontIndex);
                    writer.WriteLink(font.Name.Trim(), "fnt:" + fontIndex);
                    writer.WriteLine();
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            writer.WriteLine();
        }

        private void WriteFooter(IExtendedWriter writer)
        {
            var footer = this.file.Footer;
            if (string.IsNullOrEmpty(footer))
            {
                return;
            }

            writer.WriteLine("Footer:");
            foreach (var part in footer.Split(';'))
            {
                writer.Write(Indent);
                writer.WriteLine(part);
            }

            writer.WriteLine();
        }
    }
}
