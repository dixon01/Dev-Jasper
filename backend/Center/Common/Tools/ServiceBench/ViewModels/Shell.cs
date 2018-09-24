// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the root view model.
    /// </summary>
    [Export]
    public class Shell : ViewModelBase
    {
        /// <summary>
        /// Stores the lazy reference to the <see cref="Commands"/>.
        /// </summary>
        private readonly Lazy<Commands> commands;

        /// <summary>
        /// Stores the lazy reference to the <see cref="Operations"/> view model.
        /// </summary>
        private readonly Lazy<Operations> operations;

        /// <summary>
        /// Stores the lazy reference to the collection of tabs.
        /// </summary>
        private readonly Lazy<ObservableCollection<Tab>> tabs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="operations">The lazy reference to the operations view model.</param>
        [ImportingConstructor]
        public Shell([Import] Lazy<Commands> commands, [Import] Lazy<Operations> operations)
        {
            Contract.Ensures(this.tabs != null, "The Tabs can't be null after the initialization.");
            this.commands = commands;
            this.operations = operations;
            this.tabs = new Lazy<ObservableCollection<Tab>>(this.CreateTabsCollection);
        }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        public Operations Operations
        {
            get
            {
                return this.operations.Value;
            }
        }

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        public ObservableCollection<Tab> Tabs
        {
            get
            {
                return this.tabs.Value;
            }
        }

        /// <summary>
        /// Defines the code contract invariants.
        /// </summary>
        [ContractInvariantMethod]
        protected void ObjectInvariant()
        {
            Contract.Invariant(this.Tabs != null, "The Tabs collection can't be null.");
        }

        /// <summary>
        /// Creates the tabs collection.
        /// </summary>
        /// <returns>The observable collection of tabs.</returns>
        private ObservableCollection<Tab> CreateTabsCollection()
        {
            var tabsCollection = new ObservableCollection<Tab> { this.Operations };
            return tabsCollection;
        }
    }
}
