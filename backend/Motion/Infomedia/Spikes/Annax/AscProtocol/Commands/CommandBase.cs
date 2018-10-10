namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    using System;

    using NLog;

    public abstract class CommandBase
    {
        protected CommandBase(Command command)
        {
            this.Command = command;
        }

        public Command Command { get; private set; }

        public static CommandBase Parse(NetworkServiceType serviceType, FrameReader reader)
        {
            var command = reader.ReadUInt16();
            switch ((Command)command)
            {
                case Command.Bitmap:
                    return new BitmapCommand(reader);
                case Command.Font:
                    return new FontCommand(reader);
                case Command.Text:
                    return new TextCommand(reader);
                case Command.Window:
                    return new WindowCommand(reader);
                case Command.ClearWindow:
                    return new ClearWindowCommand(reader);
                case Command.ClearBitmap:
                    return new ClearBitmapCommand(reader);
                case Command.ClearAll:
                    return new ClearAllCommand(reader);
                case Command.GetStatus:
                    return serviceType == NetworkServiceType.Request
                               ? (CommandBase)new StatusRequest(reader)
                               : new StatusResponse(reader);
                default:
                    throw new ArgumentException(string.Format("Unknown command: {0:X4}", command));
            }
        }

        public virtual void WriteTo(FrameWriter writer)
        {
            writer.WriteUInt16((ushort)this.Command);
        }

        public override string ToString()
        {
            return this.Command.ToString();
        }
    }
}