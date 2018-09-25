// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodThird.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles.Valid
{
    /// <summary>
    /// Object tasked to represent a file with valid departures inside.
    /// </summary>
    public class GoodThird : ValidDepartureFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoodThird"/> class.
        /// </summary>
        public GoodThird()
        {
            this.Content =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<departures xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" device-id=""PC1237""
expiration=""2013-02-28 10:06"" stationname=""This is the station name"" ETA=""12:34"">
  <!--Info for Gorba Departures.
      Station=""Rotterdam, Station Lombardijen"". 
      Busnumber = 6200
      ETA = 16:08:34
      Created 16-10-2012 15:45:49
      BusTimeStamp = 16-10-2012 15:45:35
     PlaceId=6675 -->
  <busdepartures>
    <departure departuretime=""08:30"" delay=""-123"" platform=""PLATFORM"" destination=""DESTINATION"" pto=""PTO"" />
    <departure departuretime=""09:45"" delay=""+000"" platform=""no platform"" destination=""no destination"" pto=""Arriva"" />
  </busdepartures>
</departures>";
        }
    }
}
