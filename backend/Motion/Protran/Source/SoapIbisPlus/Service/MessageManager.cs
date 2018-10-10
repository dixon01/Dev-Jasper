// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    using MFD.MFDCustomerService;

    using NLog;

    /// <summary>
    /// Class that manages the passenger information messages and creates Ximple for it.
    /// </summary>
    internal class MessageManager : DataManagerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DataItemHandler bodyHandler;

        private readonly SortedMap<int, PassengerInformation> information =
            new SortedMap<int, PassengerInformation>();

        private int oldRowCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="handlerFactory">
        /// The item handler factory.
        /// </param>
        public MessageManager(PassengerInfoConfig config, ItemHandlerFactory handlerFactory)
        {
            this.bodyHandler = handlerFactory.CreateHandler(config.Body);
        }

        /// <summary>
        /// Adds passenger information.
        /// </summary>
        /// <param name="passengerInformationList">
        /// The passenger information list.
        /// </param>
        public void AddPassengerInfo(IEnumerable<PassengerInformation> passengerInformationList)
        {
            foreach (var passengerInformation in passengerInformationList)
            {
                this.information[passengerInformation.Id] = passengerInformation;
            }

            this.SendXimple();
        }

        /// <summary>
        /// Deletes the referenced passenger info.
        /// </summary>
        /// <param name="informationId">
        /// The information id.
        /// </param>
        public void DeletePassengerInfo(int informationId)
        {
            if (!this.information.Remove(informationId))
            {
                Logger.Warn("Got DeletePassengerInfo({0}), but that information ID is unknown", informationId);
                return;
            }

            this.SendXimple();
        }

        private void SendXimple()
        {
            var ximple = new Ximple();

            var rowIndex = 0;
            foreach (var passengerInformation in this.information.Values)
            {
                this.bodyHandler.AddCell(ximple, passengerInformation.Body, rowIndex++);
            }

            for (var r = rowIndex; r < this.oldRowCount; r++)
            {
                this.bodyHandler.AddCell(ximple, string.Empty, r);
            }

            this.oldRowCount = rowIndex;

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
