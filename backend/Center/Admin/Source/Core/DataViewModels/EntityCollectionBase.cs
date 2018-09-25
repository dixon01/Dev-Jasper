// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCollectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityCollectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System.Collections;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Base class for all properties that have a collection of entity references.
    /// </summary>
    public abstract class EntityCollectionBase : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionBase"/> class.
        /// </summary>
        /// <param name="displayName">
        /// The display name of this collection (i.e. the property name of the property it belongs to).
        /// </param>
        /// <param name="entityName">
        /// The entity name.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry to get commands from.
        /// </param>
        protected EntityCollectionBase(string displayName, string entityName, ICommandRegistry commandRegistry)
        {
            this.DisplayName = displayName;
            this.EntityName = entityName;
            this.commandRegistry = commandRegistry;

            this.IsLoading = true;
        }

        /// <summary>
        /// Gets the collection entities.
        /// </summary>
        public abstract ICollection Entities { get; }

        /// <summary>
        /// Gets the display name of this collection (i.e. the property name of the property it belongs to).
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this collection is currently loading.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.SetProperty(ref this.isLoading, value, () => this.IsLoading);
            }
        }

        /// <summary>
        /// Gets the navigate to entity command.
        /// </summary>
        public ICommand NavigateToEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.NavigateTo);
            }
        }
    }
}
