// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteUpdateService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteUpdateService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Messages;

    using NLog;

    using UpdateCommand = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;

    /// <summary>
    /// Wrapper for an <see cref="IUpdateService"/> designed for remote services.
    /// </summary>
    [ErrorHandler]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteUpdateService : IUpdateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IUpdateService updateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteUpdateService"/> class.
        /// </summary>
        /// <param name="updateService">The update service.</param>
        /// <exception cref="ArgumentNullException">The input service is null.</exception>
        public RemoteUpdateService(IUpdateService updateService)
        {
            if (updateService == null)
            {
                throw new ArgumentNullException("updateService");
            }

            this.updateService = updateService;
        }

        /// <summary>
        /// Asynchronously creates all update commands for all units in the given update group.
        /// </summary>
        /// <param name="updateGroupId">
        /// The id of an update group (<see cref="UpdateGroup.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for all units in the given update group.
        /// </returns>
        [XmlSerializerFormat]
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUpdateGroupAsync(int updateGroupId)
        {
            try
            {
                return await this.updateService.CreateUpdateCommandsForUpdateGroupAsync(updateGroupId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception,
                    "Error while creating update commands for update group " + updateGroupId);
                throw new FaultException(exception.Message);
            }
        }

        /// <summary>
        /// Asynchronously creates all update commands the given unit.
        /// </summary>
        /// <param name="unitId">
        /// The id of an unit (<see cref="Unit.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for the given unit.
        /// </returns>
        [XmlSerializerFormat]
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUnitAsync(int unitId)
        {
            try
            {
                return await this.updateService.CreateUpdateCommandsForUnitAsync(unitId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while creating update commands for unit " + unitId);
                throw new FaultException(exception.Message);
            }
        }

        /// <summary>
        /// Asynchronously adds update feedback from a unit to the background system database.
        /// </summary>
        /// <param name="stateInfos">
        /// The state information.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        [XmlSerializerFormat]
        public async Task AddFeedbacksAsync(UpdateStateInfo[] stateInfos)
        {
            try
            {
                await this.updateService.AddFeedbacksAsync(stateInfos);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while adding feedback");
                throw new FaultException(exception.Message);
            }
        }

        /// <summary>
        /// Asynchronously uploads a log file to the background system database.
        /// </summary>
        /// <param name="uploadRequest">
        /// The upload request containing a stream with the log file contents.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        public async Task UploadLogFileAsync(LogFileUploadRequest uploadRequest)
        {
            try
            {
                await this.updateService.UploadLogFileAsync(uploadRequest);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while uploading a log file");
                throw new FaultException(exception.Message);
            }
        }
    }
}
