// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationServiceProxy.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationServiceProxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.DTO.Activities;
    using Gorba.Center.Common.ServiceModel.DTO.Operations;
    using Gorba.Center.Common.ServiceModel.DTO.Roles;

    /// <summary>
    /// Implements a proxy for the <see cref="IOperationService"/>.
    /// </summary>
    public sealed class OperationServiceProxy : ClientBase<IOperationService>, IOperationService
    {
        /// <summary>
        /// The get operation.
        /// </summary>
        /// <param name="operationIdentifier">
        /// The operation identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Operation"/>.
        /// </returns>
        public Operation GetOperation(OperationIdentifier operationIdentifier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Submits an operation to the system.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// The identifier assigned to the operation.
        /// </returns>
        public Operation SubmitOperation(Operation operation)
        {
            var id = this.Channel.SubmitOperation(operation);
            return id;
        }

        /// <summary>
        /// The update operation.
        /// </summary>
        /// <param name="tenantIdentifier">
        /// The tenant identifier.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="Operation"/>.
        /// </returns>
        public Operation UpdateOperation(TenantIdentifier tenantIdentifier, Operation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the operation to the system.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The identifier assigned to the operation in the system.</returns>
        public Operation AddOperation(Operation operation)
        {
            var result = this.Channel.AddOperation(operation);
            return result;
        }

        /// <summary>
        /// Counts the operations in the system.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The number of operations in the system.
        /// </returns>
        public int CountOperations(FilterBase filter = null)
        {
            return this.Channel.CountOperations(filter);
        }

        /// <summary>
        /// The delete operation.
        /// </summary>
        /// <param name="operationIdentifier">
        /// The operation identifier.
        /// </param>
        public void DeleteOperation(OperationIdentifier operationIdentifier)
        {
            this.Channel.DeleteOperation(operationIdentifier);
        }

        /// <summary>
        /// The get activity.
        /// </summary>
        /// <param name="activityIdentifier">
        /// The activity identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Activity"/>.
        /// </returns>
        public Activity GetActivity(ActivityIdentifier activityIdentifier)
        {
            return this.Channel.GetActivity(activityIdentifier);
        }

        /// <summary>
        /// The list activity instances.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ActivityInstance> ListActivityInstances(FilterBase filter = null)
        {
            return this.Channel.ListActivityInstances(filter);
        }

        /// <summary>
        /// The get activity instance.
        /// </summary>
        /// <param name="activityInstanceIdentifier">
        /// The activity instance identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActivityInstance"/>.
        /// </returns>
        public ActivityInstance GetActivityInstance(ActivityInstanceIdentifier activityInstanceIdentifier)
        {
            return this.Channel.GetActivityInstance(activityInstanceIdentifier);
        }

        /// <summary>
        /// The list all operations.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Operation> ListAllOperations(FilterBase filter = null)
        {
            return this.Channel.ListAllOperations(filter);
        }

        /// <summary>
        /// Deleted an operation from the system.
        /// </summary>
        /// <param name="id">The identifier of the operation to delete.</param>
        public void DeleteOperation(int id)
        {
            this.Channel.DeleteOperation(new OperationIdentifier(id));
        }

        /// <summary>
        /// Gets the definition of an activity.
        /// </summary>
        /// <param name="id">The identifier of the activity.</param>
        /// <returns>
        /// The <see cref="Activity"/> if found; <b>null</b> otherwise.
        /// </returns>
        public Activity GetActivity(int id)
        {
            return this.Channel.GetActivity(new ActivityIdentifier(id));
        }

        /// <summary>
        /// Gets the activity instances for the specified operations.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The activity instances for the given operation.</returns>
        public IEnumerable<ActivityInstance> GetActivityInstances(FilterBase filter)
        {
            return this.Channel.ListActivityInstances(filter);
        }

        /// <summary>
        /// Gets all the operations applying filtering and paging.
        /// </summary>
        /// <param name="filter">The filter to be applied.</param>
        /// <returns>
        /// The <see cref="Operation"/>s applying to the given filters.
        /// </returns>
        public IEnumerable<Operation> GetAllOperations(FilterBase filter = null)
        {
            return this.Channel.ListAllOperations(filter);
        }

        /// <summary>
        /// Gets an operation.
        /// </summary>
        /// <param name="id">The identifier of the operation to get.</param>
        /// <returns>
        /// The <see cref="Operation"/> with the given identifier, if found; <b>null</b> otherwise.
        /// </returns>
        public Operation GetOperation(int id)
        {
            return this.Channel.GetOperation(new OperationIdentifier(id));
        }

        /// <summary>
        /// Requests the start of an operation.
        /// </summary>
        /// <param name="operationId">The identifier of the operation.</param>
        public void RequestStartOperation(int operationId)
        {
            this.Channel.RequestStartOperation(operationId);
        }

        /// <summary>
        /// Requests the stop of an operation.
        /// </summary>
        /// <param name="operationId">The identifier of the operation.</param>
        public void RequestStopOperation(int operationId)
        {
            this.Channel.RequestStopOperation(operationId);
        }

        /// <summary>
        /// Gets the activity task.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The activity task with the given identifier.</returns>
        public ActivityTask GetActivityTask(int id)
        {
            return this.Channel.GetActivityTask(id);
        }

        /// <summary>
        /// Gets the activity task.
        /// </summary>
        /// <param name="taskIdentifier">The task identifier.</param>
        /// <returns>The activity task with the given identifier.</returns>
        public ActivityTask GetActivityTask(Guid taskIdentifier)
        {
            return this.Channel.GetActivityTask(taskIdentifier);
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="updateRecord">The update record.</param>
        public void UpdateState(ActivityInstanceStateUpdateRecord updateRecord)
        {
            this.Channel.UpdateState(updateRecord);
        }

        /// <summary>
        /// Updates an operation.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant of the current operation context.</param>
        /// <param name="operation">The operation to update.</param>
        /// <returns>The updated operation.</returns>
        public Operation UpdateOperation(int tenantId, Operation operation)
        {
            return this.Channel.UpdateOperation(new TenantIdentifier(tenantId), operation);
        }
    }
}