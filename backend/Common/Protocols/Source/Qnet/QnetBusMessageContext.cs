using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorba.Common.Protocols.Qnet
{
    public class QnetBusMessageContext
    {
        public int LineId { get; set; }
        public int BlockId { get; set; }
        public int TripId { get; set; }
        public int StopId { get; set; } // = stopsNbr
        public int BusId { get; set; }
        public Byte VehicleType { get; set; } // = flags 
        public Byte Attribute { get; set; }
        public int Timestamp { get; set; }
        public int Distance { get; set; }
    }
}
