// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyDepFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles.Invalid
{
    /// <summary>
    /// Object tasked to represent a departure file with no departure inside.
    /// </summary>
    public class EmptyDepFile : InvalidDepartureFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyDepFile"/> class.
        /// </summary>
        public EmptyDepFile()
        {
            this.Content =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<departures xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    device-id=""PC1237"" expiration=""2013-02-28 19:08""
    stationname=""Rotterdam, Station Lombardijen"" ETA=""16:08"">
    <!--Info for Gorba Departures.
        Station=""Rotterdam, Station Lombardijen"". 
        Busnumber = 6200
        ETA = 16:08:34
        Created 16-10-2012 15:45:49
        BusTimeStamp = 16-10-2012 15:45:35
        PlaceId=6675 -->
    <traindepartures>
    </traindepartures>
    <busdepartures>
    </busdepartures>
</departures>";
        }
    }
}
