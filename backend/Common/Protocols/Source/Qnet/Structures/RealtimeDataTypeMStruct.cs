// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealtimeDataTypeMStruct.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Realtime monitoring display data for led matrix display (type M) - without Lane information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Realtime monitoring display data for led matrix display (type M) - without Lane information
    /// </summary>
    /// <remarks>Len = 220 bytes</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RealtimeDataTypeMStruct
    {
        private byte rowsNumber1;

        private byte rowsNumber2;

        private byte rowsNumber3;

        private byte rowsNumber4;

        private RealtimeLineDataStruct line1;

        private RealtimeLineDataStruct line2;

        private RealtimeLineDataStruct line3;

        private RealtimeLineDataStruct line4;

        private RealtimeDestinationDataStruct destination1;

        private RealtimeDestinationDataStruct destination2;

        private RealtimeDestinationDataStruct destination3;

        private RealtimeDestinationDataStruct destination4;

        private RealtimeDepartureTimeDataStruct departureTime1;

        private RealtimeDepartureTimeDataStruct departureTime2;

        private RealtimeDepartureTimeDataStruct departureTime3;

        private RealtimeDepartureTimeDataStruct departureTime4;

        /// <summary>
        /// Gets or sets a value indicating the number of rows on display.
        /// </summary>
        public byte[] RowsNumber
        {
            get
            {
                return this.GetRowNumber();
            }

            set
            {
                this.SetRowNumber(value);
            }
        }

        /// <summary>
        /// Gets or sets an array of 4 rows of <see cref="RealtimeLineDataStruct"/>.
        /// </summary>
        /// <remarks>Len = 4*10 = 40 bytes </remarks>
        public RealtimeLineDataStruct[] Line
        {
            get
            {
                return this.GetLine();
            }

            set
            {
                this.SetLine(value);
            }
        }

        /// <summary>
        /// Gets or sets an array of 4 rows of <see cref="RealtimeDepartureTimeDataStruct"/>.
        /// </summary>
        /// <remarks>Len = 4*10 = 40 bytes </remarks>
        public RealtimeDepartureTimeDataStruct[] DepartureTime
        {
            get
            {
                return this.GetDepartureTime();
            }

            set
            {
                this.SetDepartureTime(value);
            }
        }

        /// <summary>
        /// Gets or sets an array of 4 rows of <see cref="RealtimeDestinationDataStruct"/>.
        /// </summary>
        /// <remarks>Len = 34*4 = 136 bytes </remarks>        
        public RealtimeDestinationDataStruct[] Destination
        {
            get
            {
                return this.GetDestination();
            }

            set
            {
                this.SetDestination(value);
            }
        }

        private byte[] GetRowNumber()
        {
            byte[] data = { this.rowsNumber1, this.rowsNumber2, this.rowsNumber3, this.rowsNumber4 };

            return data;
        }

        private void SetRowNumber(byte[] rowNumber)
        {
            if (rowNumber.Length > 4)
            {
                throw new ArgumentOutOfRangeException("rowNumber", "The array could have a maximum of 4 entries.");
            }

            var len = rowNumber.Length;
            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0:
                        this.rowsNumber1 = rowNumber[i];
                        break;
                    case 1:
                        this.rowsNumber2 = rowNumber[i];
                        break;
                    case 2:
                        this.rowsNumber3 = rowNumber[i];
                        break;
                    case 3:
                        this.rowsNumber4 = rowNumber[i];
                        break;
                }
            }
        }

        private RealtimeLineDataStruct[] GetLine()
        {
            RealtimeLineDataStruct[] data = { this.line1, this.line2, this.line3, this.line4 };

            return data;
        }

        private void SetLine(RealtimeLineDataStruct[] line)
        {
            if (line.Length > 4)
            {
                throw new ArgumentOutOfRangeException("line", "The array could have a maximum of 4 entries.");
            }

            var len = line.Length;
            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0:
                        this.line1 = line[i];
                        break;
                    case 1:
                        this.line2 = line[i];
                        break;
                    case 2:
                        this.line3 = line[i];
                        break;
                    case 3:
                        this.line4 = line[i];
                        break;
                }
            }
        }

        private RealtimeDestinationDataStruct[] GetDestination()
        {
            RealtimeDestinationDataStruct[] data = {
                                                       this.destination1, this.destination2, this.destination3,
                                                       this.destination4
                                                   };

            return data;
        }

        private void SetDestination(RealtimeDestinationDataStruct[] destination)
        {
            if (destination.Length > 4)
            {
                throw new ArgumentOutOfRangeException("destination", "The array could have a maximum of 4 entries.");
            }

            var len = destination.Length;
            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0:
                        this.destination1 = destination[i];
                        break;
                    case 1:
                        this.destination2 = destination[i];
                        break;
                    case 2:
                        this.destination3 = destination[i];
                        break;
                    case 3:
                        this.destination4 = destination[i];
                        break;
                }
            }
        }

        private RealtimeDepartureTimeDataStruct[] GetDepartureTime()
        {
            RealtimeDepartureTimeDataStruct[] data = 
            {
                this.departureTime1, this.departureTime2, this.departureTime3, this.departureTime4
            };

            return data;
        }

        private void SetDepartureTime(RealtimeDepartureTimeDataStruct[] departureTime)
        {
            if (departureTime.Length > 4)
            {
                throw new ArgumentOutOfRangeException("departureTime", "The array could have a maximum of 4 entries.");
            }

            var len = departureTime.Length;
            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0:
                        this.departureTime1 = departureTime[i];
                        break;
                    case 1:
                        this.departureTime2 = departureTime[i];
                        break;
                    case 2:
                        this.departureTime3 = departureTime[i];
                        break;
                    case 3:
                        this.departureTime4 = departureTime[i];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            "departureTime", "The array could have a maximum of 4 entries.");
                }
            }
        }
    }
}
