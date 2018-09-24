// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoodFirst.cs" company="Gorba AG">
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
    public class GoodFirst : ValidDepartureFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoodFirst"/> class.
        /// </summary>
        public GoodFirst()
        {
            this.Content =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<departures xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" device-id=""PC1237""
    expiration=""2013-02-28 19:08"" stationname=""Dordrecht, Centraal Station (perron A)"" ETA=""15:52"">
    <!--Info for Gorba Departures.
        Station=""Dordrecht, Centraal Station (perron A)"". 
        Busnumber = 6204
        ETA = 15:52:39
        Created 16-10-2012 15:45:51
        BusTimeStamp = 16-10-2012 15:45:39
        PlaceId=6496 -->
    <traindepartures>
    <departure departuretime=""15:42"" delay=""+1"" platform=""Bäüö"" destination=""B1"" pto=""NS"" />
    <departure departuretime=""15:44"" delay="""" platform=""B"" destination=""B1"" pto=""Arriva"" />
    </traindepartures>
    <busdepartures>
    <departure departuretime=""15:44"" delay=""-4"" platform=""B"" destination=""B1"" pto=""Arriva"" StopCode=""53600150"" line=""14"" />
    <departure departuretime=""15:52"" delay=""-4"" platform=""B"" destination=""B1"" pto=""Arriva"" StopCode=""53600150"" line=""166"" />
    <departure departuretime=""15:58"" delay=""+10"" platform=""C"" destination=""B1"" pto=""Arriva"" StopCode=""53600160"" line=""165"" />
    <departure departuretime=""16:00"" delay=""+14"" platform=""B"" destination=""B1"" pto=""Arriva"" StopCode=""53600150"" line=""16"" />
    <departure departuretime=""16:08"" delay=""-2"" platform=""B"" destination=""B2"" pto=""Arriva"" StopCode=""53600150"" line=""13"" />
    <departure departuretime=""16:09"" delay=""+13"" platform=""B"" destination=""B2"" pto=""Arriva"" StopCode=""53600150"" line=""178"" />
    <departure departuretime=""16:12"" delay=""-4"" platform=""C"" destination=""B2"" pto=""Arriva"" StopCode=""53600160"" line=""12"" />
    <departure departuretime=""16:22"" delay=""+1"" platform=""B"" destination=""B2"" pto=""Arriva"" StopCode=""53600150"" line=""166"" />
    <departure departuretime=""16:22"" delay=""+9"" platform=""B"" destination=""B2"" pto=""Arriva"" StopCode=""53600150"" line=""15"" />
    <departure departuretime=""16:28"" delay=""-4"" platform=""C"" destination=""B2"" pto=""Arriva"" StopCode=""53600160"" line=""165"" />
    <departure departuretime=""16:30"" delay="""" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""16"" />
    <departure departuretime=""16:38"" delay=""+10"" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""175"" />
    <departure departuretime=""16:38"" delay=""-2"" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""13"" />
    <departure departuretime=""16:39"" delay=""+9"" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""178"" />
    <departure departuretime=""16:44"" delay=""+6"" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""14"" />
    <departure departuretime=""16:52"" delay=""+14"" platform=""B"" destination=""B3"" pto=""Arriva"" StopCode=""53600150"" line=""166"" />
    </busdepartures>
</departures>";
        }
    }
}
