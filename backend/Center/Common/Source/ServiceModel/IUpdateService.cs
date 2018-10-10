// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Messages;

    using UpdateCommandMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;

    /// <summary>
    /// Defines the service for update command download and feedback upload.
    /// </summary>
    [ServiceContract]
    public interface IUpdateService
    {
        /// <summary>
        /// Asynchronously creates all update commands for all units in the given update group.
        /// </summary>
        /// <param name="updateGroupId">
        /// The id of an update group (<see cref="UpdateGroup.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for all units in the given update group.
        /// </returns>
        [OperationContract]
        [XmlSerializerFormat]
        Task<List<UpdateCommandMsg>> CreateUpdateCommandsForUpdateGroupAsync(int updateGroupId);

        /// <summary>
        /// Asynchronously creates all update commands the given unit.
        /// </summary>
        /// <param name="unitId">
        /// The id of an unit (<see cref="Unit.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for the given unit.
        /// </returns>
        [OperationContract]
        [XmlSerializerFormat]
        Task<List<UpdateCommandMsg>> CreateUpdateCommandsForUnitAsync(int unitId);

        /// <summary>
        /// Asynchronously adds update feedback from a unit to the background system database.
        /// </summary>
        /// <param name="stateInfos">
        /// The state information.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        [OperationContract]
        [XmlSerializerFormat]
        Task AddFeedbacksAsync(UpdateStateInfo[] stateInfos);

        /// <summary>
        /// Asynchronously uploads a log file to the background system database.
        /// </summary>
        /// <param name="uploadRequest">
        /// The upload request containing a stream with the log file contents.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        [OperationContract]
        Task UploadLogFileAsync(LogFileUploadRequest uploadRequest);
    }
}
