namespace Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol
{
    using System.Text;

    public abstract class Telegram
    {
        protected Telegram(byte code, int payloadLength)
        {
            this.Code = code;
            this.PayloadLength = payloadLength;
        }

        public int PayloadLength { get; private set; }

        public byte Code { get; private set; }

        public byte[] ToByteArray()
        {
            var data = new byte[this.PayloadLength + 3];
            this.FillData(data, 2, this.PayloadLength);

            data[0] = 0x02;
            data[1] = this.Code;
            data[data.Length - 1] = 0x03;
            return data;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        protected abstract void FillData(byte[] data, int offset, int length);
    }

    public class ClearLayout : Telegram
    {
        public ClearLayout()
            : base(0xFF, 0)
        {
        }

        protected override void FillData(byte[] data, int offset, int length)
        {
            // empty telegram
        }
    }

    public class SetCell : Telegram
    {
        public SetCell()
            : base(0x10, 6)
        {
        }

        public byte CellNumber { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public CellAlignment Align { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}[Cell={6},X={1},Y={2},W={3},H={4},Align={5}]",
                this.GetType().Name,
                this.X,
                this.Y,
                this.Width,
                this.Height,
                this.Align,
                this.CellNumber);
        }

        protected override void FillData(byte[] data, int offset, int length)
        {
            data[offset++] = this.CellNumber;
            data[offset++] = this.X;
            data[offset++] = this.Y;
            data[offset++] = this.Width;
            data[offset++] = this.Height;
            data[offset] = (byte)this.Align;
        }
    }

    public class SetText : Telegram
    {
        public SetText(string text, TextAlignment align = TextAlignment.Default)
            : base(0x20, 4 + (align == TextAlignment.Default ? 0 : 1) + (text.Length * 2))
        {
            this.Text = text;
            this.Align = align;
        }

        public byte CellNumber { get; set; }

        public TextAlignment Align { get; private set; }

        public byte FontNumber { get; set; }

        public TextMode Mode { get; set; }

        public string Text { get; private set; }

        protected override void FillData(byte[] data, int offset, int length)
        {
            data[offset++] = (byte)this.PayloadLength;
            data[offset++] = this.CellNumber;
            if (this.Align != TextAlignment.Default)
            {
                data[offset++] = (byte)this.Align;
            }

            data[offset++] = this.FontNumber;
            data[offset++] = (byte)this.Mode;
            Encoding.BigEndianUnicode.GetBytes(this.Text, 0, this.Text.Length, data, offset);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}[Cell={1},Align={2},Font={3},Mode={4},Text={5}]",
                this.GetType().Name,
                this.CellNumber,
                this.Align,
                this.FontNumber,
                this.Mode,
                this.Text);
        }
    }

    public enum TextAlignment : byte
    {
        Default = 0,
        Left = 0xEF,
        Center = 0xEE,
        Right = 0xED
    }

    public enum TextMode : byte
    {
        Normal = 0,
        Blink = 1,
        ScrollRightBlank = 2,
        ScrollLeftBlank = 3,
        ScrollRightImmediate = 4,
        ScrollLeftImmediate = 5,
    }

    public enum CellAlignment : byte
    {
        Left = 0,
        Center = 1,
        Right = 2
    }
}