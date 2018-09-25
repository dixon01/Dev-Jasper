// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Editor
{
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The view model for the entity editor.
    /// </summary>
    public class EntityEditorViewModel : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private DataViewModelBase editingEntity;

        private string entityTypeName;

        private string entityTypeDisplayName;

        private bool isNewEntity;

        private bool isSaving;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public EntityEditorViewModel(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the name of the type of the entity being edited.
        /// </summary>
        public string EntityTypeName
        {
            get
            {
                return this.entityTypeName;
            }

            set
            {
                this.SetProperty(ref this.entityTypeName, value, () => this.EntityTypeName);
            }
        }

        /// <summary>
        /// Gets or sets the display name of the type of the entity being edited.
        /// </summary>
        public string EntityTypeDisplayName
        {
            get
            {
                return this.entityTypeDisplayName;
            }

            set
            {
                this.SetProperty(ref this.entityTypeDisplayName, value, () => this.EntityTypeDisplayName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity being edited is new
        /// (i.e. doesn't exist yet in the database).
        /// </summary>
        public bool IsNewEntity
        {
            get
            {
                return this.isNewEntity;
            }

            set
            {
                this.SetProperty(ref this.isNewEntity, value, () => this.IsNewEntity);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is currently saving the entity being edited.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return this.isSaving;
            }

            set
            {
                this.SetProperty(ref this.isSaving, value, () => this.IsSaving);
            }
        }

        /// <summary>
        /// Gets or sets the entity that is currently being edited.
        /// This property might be null if no entity is editing at the moment.
        /// </summary>
        public DataViewModelBase EditingEntity
        {
            get
            {
                return this.editingEntity;
            }

            set
            {
                this.SetProperty(ref this.editingEntity, value, () => this.EditingEntity);
            }
        }

        /// <summary>
        /// Gets the save entity command.
        /// </summary>
        public ICommand SaveEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Editor.Save);
            }
        }

        /// <summary>
        /// Gets the create entity command.
        /// </summary>
        public ICommand CreateEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Editor.Create);
            }
        }

        /// <summary>
        /// Gets the cancel edit command.
        /// </summary>
        public ICommand CancelEditCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Editor.CancelEdit);
            }
        }

        /// <summary>
        /// Gets the command to navigate to a given entity.
        /// </summary>
        public ICommand NavigateToEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.NavigateTo);
            }
        }

        /// <summary>
        /// Gets the command to update the property display.
        /// </summary>
        public ICommand UpdatePropertyDisplayCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Editor.UpdatePropertyDisplay);
            }
        }
    }
}
