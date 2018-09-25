namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    using System.Text;

    public class TextCommand : CommandBase
    {
        private static readonly Encoding StandardEncoding = Encoding.GetEncoding("iso-8859-1");

        public TextCommand()
            : base(Command.Text)
        {
        }

        public TextCommand(FrameReader reader)
            : this()
        {
            this.BitmapNumber = reader.ReadUInt16();
            this.Width = reader.ReadUInt16();
            this.Height = reader.ReadUInt16();
            var spacingAttribute = reader.ReadByte();
            this.Spacing = spacingAttribute >> 4;
            this.TextAttribute = (TextAttr)(spacingAttribute & 0x0F);
            this.FontNumber = reader.ReadSByte();

            this.TextData = reader.ReadRemainingBytes();
        }

        public int BitmapNumber { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Spacing { get; set; }

        public TextAttr TextAttribute { get; set; }

        public int FontNumber { get; set; }

        public byte[] TextData { get; set; }

        public override void WriteTo(FrameWriter writer)
        {
            base.WriteTo(writer);

            writer.WriteUInt16((ushort)this.BitmapNumber);
            writer.WriteUInt16((ushort)this.Width);
            writer.WriteUInt16((ushort)this.Height);
            var spacingAttribute = (int)this.TextAttribute | (this.Spacing << 4);
            writer.WriteByte((byte)spacingAttribute);
            writer.WriteSByte((sbyte)this.FontNumber);
            writer.WriteData(this.TextData);
        }

        public override string ToString()
        {
            string text;
            if (this.TextData.Length > 0 && this.TextData[this.TextData.Length - 1] == 0)
            {
                text = StandardEncoding.GetString(this.TextData, 0, this.TextData.Length - 1);
            }
            else
            {
                text = StandardEncoding.GetString(this.TextData);
            }

            return string.Format(
                "Text: bmp={0} size={1}x{2} sp={3} attr={4} font={5}, text=\"{6}\"",
                this.BitmapNumber,
                this.Width,
                this.Height,
                this.Spacing,
                this.TextAttribute,
                this.FontNumber,
                text);
        }
    }
}