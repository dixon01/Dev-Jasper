// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operations.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// The operations.
    /// </summary>
    [Export]
    public class Operations : Tab
    {
        /// <summary>
        /// Stores the lazy reference to the "GetAll" command.
        /// </summary>
        private readonly Lazy<Commands> commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="Operations"/> class.
        /// </summary>
        /// <param name="commands">The get all command.</param>
        [ImportingConstructor]
        public Operations([Import] Lazy<Commands> commands)
        {
            this.commands = commands;
            this.Items = new ObservableCollection<Operation>();
        }

        /// <summary>
        /// Gets Description.
        /// </summary>
        public override object Description
        {
            get
            {
                return "Operations services";
            }
        }

        /// <summary>
        /// Gets Header.
        /// </summary>
        public override object Header
        {
            get
            {
                return "Operations";
            }
        }

        /// <summary>
        /// Gets the command to add an operation.
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return this.commands.Value.AddOperationCommand;
            }
        }

        /// <summary>
        /// Gets the command to get all operations.
        /// </summary>
        public ICommand GetAllCommand
        {
            get
            {
                return this.commands.Value.GetAllOperationsCommand;
            }
        }

        /// <summary>
        /// Gets the operation entities.
        /// </summary>
        public ObservableCollection<Operation> Items { get; private set; }

        /// <summary>
        /// Defines the code contract invariants.
        /// </summary>
        [ContractInvariantMethod]
        protected void ObjectInvariant()
        {
            Contract.Invariant(this.Items != null, "The Items collection can't be null.");
        }
    }
}