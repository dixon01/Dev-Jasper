// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusMessageAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates the available values for bus message attribute
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates the available values for bus message attribute
    /// Definitions for attribute field of tBusMsg
    /// </summary>
    public class BusMessageAttribute
    {
        /// <summary>
        /// RBL type = 11 in decimal for iqube. In the initial documentation was 0x11 instead of 0x0B
        /// </summary>
        public const byte AttributeBusRbl = 0x0B; 

        // attributes for VCU bus messages:
        public static byte ATTR_BUS_HALTED = 0x01;
        public static byte ATTR_STATION_ENTRY = 0x02;
        public static byte ATTR_STATION_EXIT = 0x03;
        public static byte ATTR_BUS_POSITION = 0x04;
        public static byte ATTR_BUS_MELDEPUNKT = 0x0A;

        // attributes for IQUBE bus messages:
        public static byte ATTR_BUS_DEPARTURE = 0x05;
        public static byte ATTR_BUS_DELAY = 0x06;
        public static byte ATTR_BUS_DETECTED = 0x07;
        public static byte ATTR_BUS_MISSING = 0x08;
        public static byte ATTR_BUS_PASSED = 0x09;
 
        // attributes for vehicle bus messages:
        public static byte ATTR_BUS_ENTRY = 0x20;
        public static byte ATTR_BUS_EXIT = 0x21;

        /// <summary>
        /// intermediate message between stations
        /// </summary>
        public static byte ATTR_BUS_INTERMEDIATE = 0x22;

        /// <summary>
        /// intermediate message between stations indicating a blockade
        /// </summary>
        public static byte ATTR_BUS_BLOCKADE = 0x23;

        /// <summary>
        /// trip abort message (aborted by vehicle)
        /// </summary>
        public static byte ATTR_BUS_ABORT = 0x24;
    }
}
