// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// View model representing a leaf in the unit configurator navigation tree.
    /// </summary>
    public abstract class PartViewModelBase : UnitConfigTreeNodeViewModelBase
    {
        private ICommandRegistry commandRegistry;

        private bool isVisible;

        private ErrorState errorState;

        private CategoryViewModel category;

        private bool wasVisited;

        /// <summary>
        /// Gets or sets a value indicating whether this part is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (this.SetProperty(ref this.isVisible, value, () => this.IsVisible))
                {
                    // often the property is set "asynchronously" (without user interaction)
                    // therefore we need to trigger the command manager
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets the part key.
        /// This property should only be used by the controllers.
        /// </summary>
        public string PartKey { get; set; }

        /// <summary>
        /// Gets the category to which this part belongs.
        /// </summary>
        public CategoryViewModel Category
        {
            get
            {
                return this.category;
            }
        }

        /// <summary>
        /// Gets the error state of this node.
        /// </summary>
        public override ErrorState ErrorState
        {
            get
            {
                return this.errorState;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this part was visited before.
        /// This property is set when the part is no longer selected (i.e. when "leaving" the part).
        /// </summary>
        public bool WasVisited
        {
            get
            {
                return this.wasVisited;
            }

            set
            {
                this.SetProperty(ref this.wasVisited, value, () => this.WasVisited);
            }
        }

        /// <summary>
        /// Gets the command that will navigate to the given part.
        /// </summary>
        public ICommand NavigateToPartCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPart);
            }
        }

        /// <summary>
        /// Sets this view model up properly.
        /// This method shouldn't be called from outside this namespace.
        /// </summary>
        /// <param name="ownerCategory">
        /// The category owning this part.
        /// </param>
        /// <param name="registry">
        /// The command registry.
        /// </param>
        internal void Setup(CategoryViewModel ownerCategory, ICommandRegistry registry)
        {
            this.SetProperty(ref this.category, ownerCategory, () => this.Category);
            this.commandRegistry = registry;
        }

        /// <summary>
        /// Sets the <see cref="ErrorState"/>.
        /// </summary>
        /// <param name="state">
        /// The new state.
        /// </param>
        protected void SetErrorState(ErrorState state)
        {
            this.SetProperty(ref this.errorState, state, () => this.ErrorState);
        }
    }
}