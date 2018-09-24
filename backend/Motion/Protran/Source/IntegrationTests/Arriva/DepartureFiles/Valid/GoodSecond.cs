// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodSecond.cs" company="Gorba AG">
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
    public class GoodSecond : ValidDepartureFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoodSecond"/> class.
        /// </summary>
        public GoodSecond()
        {
            this.Content =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<departures xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" device-id=""PC1237""
expiration=""2013-02-28 10:06"" stationname=""Rotterdam, Station Lombardijen"" ETA=""16:08"">
  <!--Info for Gorba Departures.
      Station=""Rotterdam, Station Lombardijen"". 
      Busnumber = 6200
      ETA = 16:08:34
      Created 16-10-2012 15:45:49
      BusTimeStamp = 16-10-2012 15:45:35
     PlaceId=6675 -->
  <traindepartures>
    <departure departuretime=""15:58"" delay=""+1"" platform=""A"" destination=""A1"" pto=""NS"" />
    <departure departuretime=""16:00"" delay="""" platform=""B"" destination=""A1"" pto=""Arriva"" />
  </traindepartures>
  <busdepartures>
    <departure departuretime=""15:59"" delay=""-4"" platform="""" destination=""A1"" pto=""Arriva"" StopCode=""53240290"" line=""163"" />
    <departure departuretime=""16:06"" delay=""-4"" platform="""" destination=""A1"" pto=""Arriva"" StopCode=""53240290"" line=""90"" />
    <departure departuretime=""16:10"" delay=""+7"" platform="""" destination=""A1"" pto=""Arriva"" StopCode=""53141060"" line=""90"" />
    <departure departuretime=""16:12"" delay=""+8"" platform="""" destination=""A1"" pto=""Arriva"" StopCode=""53141060"" line=""91"" />
    <departure departuretime=""16:17"" delay=""+5"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53240290"" line=""92"" />
    <departure departuretime=""16:20"" delay=""-4"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53141060"" line=""92"" />
    <departure departuretime=""16:21"" delay=""+9"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53141060"" line=""163"" />
    <departure departuretime=""16:23"" delay=""+6"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53240290"" line=""91"" />
    <departure departuretime=""16:29"" delay=""+8"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53240290"" line=""163"" />
    <departure departuretime=""16:36"" delay=""+2"" platform="""" destination=""A2"" pto=""Arriva"" StopCode=""53240290"" line=""90"" />
    <departure departuretime=""16:40"" delay=""-1"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53141060"" line=""90"" />
    <departure departuretime=""16:42"" delay=""+3"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53141060"" line=""91"" />
    <departure departuretime=""16:51"" delay=""+2"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53141060"" line=""163"" />
    <departure departuretime=""16:53"" delay=""-4"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53240290"" line=""91"" />
    <departure departuretime=""16:59"" delay=""+2"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53240290"" line=""163"" />
    <departure departuretime=""17:06"" delay=""-4"" platform="""" destination=""A3"" pto=""Arriva"" StopCode=""53240290"" line=""90"" />
  </busdepartures>
</departures>";
        }
    }
}
