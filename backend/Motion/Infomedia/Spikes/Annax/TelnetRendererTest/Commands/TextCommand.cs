namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Commands
{
    using System.Text;

    public class TextCommand : CommandBase
    {
        private readonly int bitmapNumber;

        private readonly AnnaxFont font;

        private readonly Align align;

        private readonly string text;

        public enum Align
        {
            Left,
            Center,
            Right
        }

        public TextCommand(int bitmapNumber, AnnaxFont font, string text, Align align = Align.Left)
        {
            this.bitmapNumber = bitmapNumber;
            this.font = font;
            this.align = align;
            this.text = text;
        }

        public override void AppendCommandString(StringBuilder buffer)
        {
            buffer.AppendFormat(
                "text {0} {1} {2:D} \"{3}\"\n", this.bitmapNumber, this.font.Id, this.align, this.text.Replace("\"", string.Empty));
        }
    }
}