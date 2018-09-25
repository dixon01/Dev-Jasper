namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    using System;

    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(CommandBase command)
        {
            this.Command = command;
        }

        public CommandBase Command { get; private set; }
    }
}