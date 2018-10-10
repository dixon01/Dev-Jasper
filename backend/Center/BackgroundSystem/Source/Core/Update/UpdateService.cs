// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Messages;

    using UpdateCommand = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;

    /// <summary>
    /// The update service implementation.
    /// This class is basically a wrapper around <see cref="UpdateCommandManager"/> that allows
    /// to use a bit a different interface on WCF that it is used locally.
    /// </summary>
    public class UpdateService : IUpdateService, IDisposable
    {
        private readonly UpdateTransferController updateTransferController;

        private readonly UpdateDataObserver updateDataObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        public UpdateService()
        {
            this.updateTransferController = new UpdateTransferController();
            this.updateDataObserver = new UpdateDataObserver(this.updateTransferController);
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
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUpdateGroupAsync(int updateGroupId)
        {
            return await this.updateTransferController.CreateUpdateCommandsForUpdateGroupAsync(updateGroupId);
        }

        /// <summary>
        /// Asynchronously creates all update commands the given unit.
        /// </summary>
        /// <param name="unitId">
        /// The id of an unit (<see cref="Common.ServiceModel.Units.Unit.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for the given unit.
        /// </returns>
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUnitAsync(int unitId)
        {
            return await this.updateTransferController.CreateUpdateCommandsForUnitAsync(unitId);
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
        public async Task AddFeedbacksAsync(UpdateStateInfo[] stateInfos)
        {
            await this.updateTransferController.AddFeedbacksAsync(stateInfos);
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
            await
                this.updateTransferController.SaveLogFileAsync(
                    uploadRequest.Filename,
                    uploadRequest.Content,
                    uploadRequest.UnitId);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.updateDataObserver.Dispose();
            this.updateTransferController.Dispose();
        }
    }
}
