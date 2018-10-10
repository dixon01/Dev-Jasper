namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class WindowCommand : CommandBase
    {
        public WindowCommand()
            : base(Command.Window)
        {
        }

        public WindowCommand(FrameReader reader)
            : this()
        {
            this.WindowNumber = reader.ReadUInt16();
            this.X = reader.ReadUInt16();
            this.Y = reader.ReadUInt16();
            this.Width = reader.ReadUInt16();
            this.Height = reader.ReadUInt16();
            this.StartX = reader.ReadUInt16();
            this.StartY = reader.ReadUInt16();
            this.BitmapNumber = reader.ReadUInt16();
            this.BaseAttribute = (BaseAttr)reader.ReadUInt16();
            this.DurationAttribute = (DurationAttr)reader.ReadUInt16();
            this.DisplayAttribute = (DisplayAttr)reader.ReadUInt16();
            this.Timing = reader.ReadUInt16();
            this.Counting = reader.ReadUInt16();
        }

        public int WindowNumber { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }

        public int BitmapNumber { get; set; }
        public BaseAttr BaseAttribute { get; set; }
        public DurationAttr DurationAttribute { get; set; }
        public DisplayAttr DisplayAttribute { get; set; }
        public int Timing { get; set; }
        public int Counting { get; set; }

        public override void WriteTo(FrameWriter writer)
        {
            base.WriteTo(writer);

            writer.WriteUInt16((ushort)this.WindowNumber);
            writer.WriteUInt16((ushort)this.X);
            writer.WriteUInt16((ushort)this.Y);
            writer.WriteUInt16((ushort)this.Width);
            writer.WriteUInt16((ushort)this.Height);
            writer.WriteUInt16((ushort)this.StartX);
            writer.WriteUInt16((ushort)this.StartY);
            writer.WriteUInt16((ushort)this.BitmapNumber);
            writer.WriteUInt16((ushort)this.BaseAttribute);
            writer.WriteUInt16((ushort)this.DurationAttribute);
            writer.WriteUInt16((ushort)this.DisplayAttribute);
            writer.WriteUInt16((ushort)this.Timing);
            writer.WriteUInt16((ushort)this.Counting);
        }

        public override string ToString()
        {
            return string.Format(
                "Window: win={0} bmp={7} pos=[{1},{2}] size={3}x{4} start=[{5},{6}] " +
                "baseAttr={8} duration={9} display={10} timing={11} counting={12}",
                this.WindowNumber,
                this.X,
                this.Y,
                this.Width,
                this.Height,
                this.StartX,
                this.StartY,
                this.BitmapNumber,
                this.BaseAttribute,
                this.DurationAttribute,
                this.DisplayAttribute,
                this.Timing,
                this.Counting);
        }
    }
}