// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCustomerInformationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestCustomerInformationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Tests
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// Test implementation of <see cref="ICustomerInformationService"/>.
    /// </summary>
    public class TestCustomerInformationService : CustomerInformationServiceBase
    {
        private CustomerInformationServiceAllData allData;

        private CustomerInformationServiceCurrentAnnouncementData currentAnnouncement;

        private CustomerInformationServiceCurrentStopIndexData currentStopIndex;

        private CustomerInformationServiceTripData tripData;

        private CustomerInformationServiceVehicleData vehicleData;

        /// <summary>
        /// Sets all data.
        /// <seealso cref="GetAllData"/>
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void SetAllData(CustomerInformationServiceAllData data)
        {
            this.allData = data;
            this.RaiseAllDataChanged();
        }

        /// <summary>
        /// Gets all data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceAllData"/>.
        /// </returns>
        public override CustomerInformationServiceAllData GetAllData()
        {
            return this.allData;
        }

        /// <summary>
        /// Sets the current announcement.
        /// <seealso cref="GetCurrentAnnouncement"/>
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void SetCurrentAnnouncement(CustomerInformationServiceCurrentAnnouncementData data)
        {
            this.currentAnnouncement = data;
            this.RaiseCurrentAnnouncementChanged();
        }

        /// <summary>
        /// Gets the current announcement.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentAnnouncementData"/>.
        /// </returns>
        public override CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement()
        {
            return this.currentAnnouncement;
        }

        /// <summary>
        /// Gets the current connection information.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentConnectionInformationData"/>.
        /// </returns>
        public override CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current display content.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentDisplayContentData"/>.
        /// </returns>
        public override CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current stop point.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentStopPointData"/>.
        /// </returns>
        public override CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current stop index.
        /// <seealso cref="GetCurrentStopIndex"/>
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void SetCurrentStopIndex(CustomerInformationServiceCurrentStopIndexData data)
        {
            this.currentStopIndex = data;
            this.RaiseCurrentStopIndexChanged();
        }

        /// <summary>
        /// Gets the current stop point index.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentStopIndexData"/>.
        /// </returns>
        public override CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex()
        {
            return this.currentStopIndex;
        }

        /// <summary>
        /// Sets the current trip data.
        /// <seealso cref="GetTripData"/>
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void SetTripData(CustomerInformationServiceTripData data)
        {
            this.tripData = data;
            this.RaiseTripDataChanged();
        }

        /// <summary>
        /// Gets the current trip data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceTripData"/>.
        /// </returns>
        public override CustomerInformationServiceTripData GetTripData()
        {
            return this.tripData;
        }

        /// <summary>
        /// Sets the current announcement.
        /// <seealso cref="GetCurrentAnnouncement"/>
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void SetVehicleData(CustomerInformationServiceVehicleData data)
        {
            this.vehicleData = data;
            this.RaiseVehicleDataChanged();
        }

        /// <summary>
        /// Gets the vehicle data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceVehicleData"/>.
        /// </returns>
        public override CustomerInformationServiceVehicleData GetVehicleData()
        {
            return this.vehicleData;
        }

        /// <summary>
        /// Retrieves a partial stop sequence.
        /// </summary>
        /// <param name="startingStopIndex">
        /// The starting stop index.
        /// </param>
        /// <param name="numberOfStopPoints">
        /// The number of stop points.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerInformationServicePartialStopSequenceData"/>.
        /// </returns>
        public override CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(
            IBISIPint startingStopIndex, IBISIPint numberOfStopPoints)
        {
            throw new NotImplementedException();
        }
    }
}