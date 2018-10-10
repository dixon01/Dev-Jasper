namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class StatusResponse : CommandBase
    {
        public StatusResponse()
            : base(Command.GetStatus)
        {
        }

        public StatusResponse(FrameReader reader)
            : this()
        {
            this.Status = reader.ReadUInt16();
            this.SystemStatus = reader.ReadUInt16();
            this.CommandCode = reader.ReadUInt16();
            this.ErrorCode = reader.ReadUInt16();
        }

        public int Status { get; set; }

        public int SystemStatus { get; set; }

        public int CommandCode { get; set; }

        public int ErrorCode { get; set; }

        public override void WriteTo(FrameWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteUInt16((ushort)this.Status);
            writer.WriteUInt16((ushort)this.SystemStatus);
            writer.WriteUInt16((ushort)this.CommandCode);
            writer.WriteUInt16((ushort)this.ErrorCode);
        }

        public override string ToString()
        {
            return string.Format(
                "StatusResponse: stat={0} sys={1} cmd={2} err={3}",
                this.Status,
                this.SystemStatus,
                this.CommandCode,
                this.ErrorCode);
        }
    }
}