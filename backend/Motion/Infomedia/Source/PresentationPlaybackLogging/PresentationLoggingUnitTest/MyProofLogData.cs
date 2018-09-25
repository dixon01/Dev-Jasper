
namespace Luminator.PresentationLogging.UnitTest
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Core.Interfaces;

    internal class MyProofLogData : IPresentationInfo
    {
        public MyProofLogData()
        {
            this.Created = DateTime.Now;
        }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public string FileName { get; set; }

        public string Trip { get; set; }

        public bool IsValid { get; }

        public string StartedLatitude { get; set; }

        public string StartedLongitude { get; set; }

        public string StoppedLatitude { get; set; }

        public string StoppedLongitude { get; set; }

        public string ResourceId { get; set; }

        public string Route { get; set; }

        public string UnitName { get; set; }

        public string VehicleId { get; set; }

        public string MyRandomData { get; set; }
    }
}
