// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDataTypeCStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Data for Realtime monitoring of displayed data for led countdown display (type C)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Data for Realtime monitoring of displayed data for led countdown display (type C)
    /// </summary>
    /// <remarks>Len = 32 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RealtimeDataTypeCStruct
    {
        private DmcDisplayDataStruct data1;       
        private DmcDisplayDataStruct data2;
        private DmcDisplayDataStruct data3;
        private DmcDisplayDataStruct data4;
        private DmcDisplayDataStruct data5;
        private DmcDisplayDataStruct data6; 
        private DmcDisplayDataStruct data7;
        private DmcDisplayDataStruct data8;

        /// <summary>
        /// Gets or sets an array of 8 rows of DmcDisplayDataStruct (4bytes)
        /// </summary>
        /// <remarks>Len = 8*4 = 32 bytes </remarks>
        public DmcDisplayDataStruct[] Data
        {
            get
            {
                return this.GetData();
            }

            set
            {
                this.SetData(value);
            }
        } 

        private DmcDisplayDataStruct[] GetData()
        {
            DmcDisplayDataStruct[] data = { this.data1, this.data2, this.data3, this.data4, this.data5, this.data6, this.data7, this.data8 };

            return data;
        }

        private void SetData(DmcDisplayDataStruct[] data)
        {
            if (data.Length > 8)
            {
                throw new ArgumentOutOfRangeException("data", "The array could have a maximum of 8 entries.");
            }

            var len = data.Length;
            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0:
                        this.data1 = data[i];
                        break;
                    case 1:
                        this.data2 = data[i];
                        break;
                    case 2:
                        this.data3 = data[i];
                        break;
                    case 3:
                        this.data4 = data[i];
                        break;
                    case 4:
                        this.data5 = data[i];
                        break;
                    case 5:
                        this.data6 = data[i];
                        break;
                    case 6:
                        this.data7 = data[i];
                        break;
                    case 7:
                        this.data8 = data[i];
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Data for Realtime monitoring of displayed data for led countdown display (type C)
    /// </summary>
    /// <remarks>Len = 4 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DmcDisplayDataStruct
    {
        /// <summary>
        /// Display values
        /// </summary>
        /// <remarks>Len = 2 bytes</remarks>
        public DisplayValueStruct DisplayValue;

        /// <summary>
        /// Display attributes: Bit 1: blink if set
        /// </summary>
        public ushort Attributes;
    }

    /// <summary>
    /// Displayed values for led countdown display splitted into units and tens.
    /// The reason to split in units and tens is to be able to send some special characters 
    /// like dashes and the letters A, E, ... e.g. it is possible to display an error code like [E1] on the display.
    /// See <see cref="RealtimeSpecialCharacters"/> for the list of avaible special characters.
    /// </summary>
    /// <remarks>Len = 2 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]public struct DisplayValueStruct
    {
        /// <summary>
        /// Tens position. (Legacy code : pos_ten)
        /// </summary>
        public sbyte PositionTens;

        /// <summary>
        /// Units position (Legacy code : pos_one)
        /// </summary>
        public sbyte PositionUnits;
    }
}
