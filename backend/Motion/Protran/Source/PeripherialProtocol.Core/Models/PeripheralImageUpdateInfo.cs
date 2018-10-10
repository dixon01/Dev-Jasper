namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;

    public class PeripheralImageUpdateInfo : EventArgs
    {
        public PeripheralImageUpdateInfo()
        {
            this.Status = string.Empty;
            this.TotalRecords = 0;
            this.Record = 0;
        }

        public PeripheralImageUpdateInfo(int totalRecords, int record) : this(string.Empty, totalRecords, record)
        {
        }

        public PeripheralImageUpdateInfo(string status, int totalRecords, int record)
        {
            this.Status = status;
            this.TotalRecords = totalRecords;
            this.Record = record;
        }

        string Status { get; set; }
        int TotalRecords { get; set; }
        int Record { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} of {1}", this.Status, this.Record, this.TotalRecords);
        }
    }
}
