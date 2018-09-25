// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Commands.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Commands type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceBench.ViewModels;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.DTO.Activities;
    using Gorba.Center.Common.ServiceModel.DTO.Roles;
    using Gorba.Center.Common.ServiceModel.DTO.Units;
    using Gorba.Center.Common.Wpf.Core;

    using Microsoft.Practices.ServiceLocation;

    using Operation = Gorba.Center.Common.ServiceBench.ViewModels.Operation;

    /// <summary>
    /// Defines the commands available in the application.
    /// </summary>
    [Export]
    public class Commands
    {
        /// <summary>
        /// The <see cref="TaskScheduler"/> used to execute operations on the UI thread.
        /// </summary>
        private readonly TaskScheduler taskScheduler;

        /// <summary>
        /// Stores the lazy reference to the <see cref="Shell"/>.
        /// </summary>
        private readonly Lazy<Shell> shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="Commands"/> class.
        /// </summary>
        /// <param name="taskScheduler">The UI task scheduler.</param>
        /// <param name="shell">The shell.</param>
        [ImportingConstructor]
        public Commands([Import] TaskScheduler taskScheduler, [Import] Lazy<Shell> shell)
        {
            this.taskScheduler = taskScheduler;
            this.shell = shell;
            this.AddOperationCommand = this.CreateCommand(this.AddOperation, this.CanExecuteAddOperation);
            this.GetAllOperationsCommand = this.CreateCommand(this.GetAllOperations, this.CanExecuteGetAll);
        }

        /// <summary>
        /// Gets the command to add an operation.
        /// </summary>
        public ICommand AddOperationCommand { get; private set; }

        /// <summary>
        /// Gets the command to get all the operations.
        /// </summary>
        public ICommand GetAllOperationsCommand { get; private set; }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public Shell Shell
        {
            get
            {
                return this.shell.Value;
            }
        }

        /// <summary>
        /// Invokes the SubmitOperation operation on the <see cref="IOperationService"/>.
        /// </summary>
        private void AddOperation()
        {
            var operationService = ServiceLocator.Current.GetInstance<IOperationService>();
            const string NameFormat = "Operation {0}";
            var name = string.Format(NameFormat, DateTime.Now);
            var user = new User { Id = 1 };
            var operation = new ServiceModel.DTO.Operations.Operation { Name = name, User = user };
            operation.Units.Add(new Unit { Id = 1 });
            operation.Activities.Add(new ActivityInfoText { DisplayText = "text" });
            Action execute = () => operationService.SubmitOperation(operation);
            var task = new Task(execute);
            task.ContinueWith(
                this.OnVoidOperationServiceCompleted,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Current);
            task.ContinueWith(
                this.OnTaskFaulted, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, this.taskScheduler);
            task.Start();
        }

        /// <summary>
        /// Determines whether it is possible to execute the SubmitOperation command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if it is possible to execute the SubmitOperation command; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteAddOperation(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Invokes the GetAll operations on the <see cref="IOperationService"/>.
        /// By default, it uses UserId 1 and takes first 10 entities.
        /// </summary>
        private void GetAllOperations()
        {
            var operationService = ServiceLocator.Current.GetInstance<IOperationService>();
            var filter = new FilterBase { UserId = 1, Take = 10 };
            Func<IEnumerable<ServiceModel.DTO.Operations.Operation>> execute =
                () => operationService.ListAllOperations(filter);
            var task = new Task<IEnumerable<ServiceModel.DTO.Operations.Operation>>(execute);
            task.ContinueWith(
                this.OnGetAllOperationsServiceCallCompleted,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                this.taskScheduler);
            task.ContinueWith(
                this.OnTaskFaulted, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, this.taskScheduler);
            task.Start();
        }

        /// <summary>
        /// Called when the call to the service completed.
        /// </summary>
        /// <param name="previousTask">The previous task.</param>
        private void OnGetAllOperationsServiceCallCompleted(
            Task<IEnumerable<ServiceModel.DTO.Operations.Operation>> previousTask)
        {
            this.Shell.Operations.Items.Clear();
            Action<ServiceModel.DTO.Operations.Operation> add = o =>
                {
                    var operation = new Operation { Id = o.Id, Name = o.Name };
                    this.Shell.Operations.Items.Add(operation);
                };
            previousTask.Result.ToList().ForEach(add);
        }

        /// <summary>
        /// Called when the call to the service completed.
        /// </summary>
        /// <param name="previousTask">The previous task.</param>
        private void OnVoidOperationServiceCompleted(Task previousTask)
        {
            this.GetAllOperations();
        }

        /// <summary>
        /// Determines whether the "GetAllOperations" command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this command "GetAllOperations" can be executed; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteGetAll(object parameter)
        {
            // TODO: implement the logic to verify that the command can be executed.
            return true;
        }

        /// <summary>
        /// Creates the command wrapping the action with an asynchronous logic.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns>The command which executed the action.</returns>
        private ICommand CreateCommand(Action execute, Predicate<object> canExecute)
        {
            var command = new RelayCommand(execute, canExecute);
            return command;
        }

        /// <summary>
        /// Called when a task was faulted.
        /// </summary>
        /// <param name="previousTask">The previous task.</param>
        private void OnTaskFaulted(Task previousTask)
        {
            if (previousTask.IsFaulted)
            {
                throw new ApplicationException(
                    "An error occurred while executing a command. Please check the inner exception for details.",
                    previousTask.Exception);
            }
        }
    }
}
