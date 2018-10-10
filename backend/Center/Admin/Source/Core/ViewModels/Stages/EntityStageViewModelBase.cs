// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityStageViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityStageViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Base class for all stages that represent lists of entities.
    /// </summary>
    public abstract class EntityStageViewModelBase : StageViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private bool isLoading;

        private string pluralDisplayName;

        private string singularDisplayName;

        private string entityName;

        private bool canCreate;
        private bool canRead;
        private bool canWrite;
        private bool canDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityStageViewModelBase"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        protected EntityStageViewModelBase(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the display name (in plural) which is used as a title of the stage.
        /// </summary>
        public string PluralDisplayName
        {
            get
            {
                return this.pluralDisplayName;
            }

            set
            {
                this.SetProperty(ref this.pluralDisplayName, value, () => this.PluralDisplayName);
            }
        }

        /// <summary>
        /// Gets or sets the display name (in singular) which is used as a title of the editor.
        /// </summary>
        public string SingularDisplayName
        {
            get
            {
                return this.singularDisplayName;
            }

            set
            {
                this.SetProperty(ref this.singularDisplayName, value, () => this.SingularDisplayName);
            }
        }

        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        public string EntityName
        {
            get
            {
                return this.entityName;
            }

            set
            {
                this.SetProperty(ref this.entityName, value, () => this.EntityName);
            }
        }

        /// <summary>
        /// Gets the list of instances. Implementations should return a typed
        /// <see cref="ObservableCollection{T}"/> in this property.
        /// </summary>
        public abstract IList Instances { get; }

        /// <summary>
        /// Gets or sets the selected instance from the <see cref="Instances"/> collection.
        /// </summary>
        public abstract ReadOnlyDataViewModelBase SelectedInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the currently logged in user can create entities.
        /// </summary>
        public bool CanCreate
        {
            get
            {
                return this.canCreate;
            }

            set
            {
                if (this.SetProperty(ref this.canCreate, value, () => this.CanCreate))
                {
                    this.RaisePropertyChanged(() => this.IsAllowed);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently logged in user can read entities.
        /// </summary>
        public bool CanRead
        {
            get
            {
                return this.canRead;
            }

            set
            {
                if (this.SetProperty(ref this.canRead, value, () => this.CanRead))
                {
                    this.RaisePropertyChanged(() => this.IsAllowed);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently logged in user can write entities.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this.canWrite;
            }

            set
            {
                if (this.SetProperty(ref this.canWrite, value, () => this.CanWrite))
                {
                    this.RaisePropertyChanged(() => this.IsAllowed);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently logged in user can delete entities.
        /// </summary>
        public bool CanDelete
        {
            get
            {
                return this.canDelete;
            }

            set
            {
                if (this.SetProperty(ref this.canDelete, value, () => this.CanDelete))
                {
                    this.RaisePropertyChanged(() => this.IsAllowed);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to this stage is allowed.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                return this.CanCreate || this.CanRead || this.CanWrite || this.CanDelete;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this stage is loading its data.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                if (this.SetProperty(ref this.isLoading, value, () => this.IsLoading) && !value)
                {
                    // often the property is reset "asynchronously" (without user interaction)
                    // therefore we need to trigger the command manager
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type of entity has details.
        /// </summary>
        public abstract bool HasDetails { get; }

        /// <summary>
        /// Gets the add entity command.
        /// </summary>
        public ICommand AddEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.Add);
            }
        }

        /// <summary>
        /// Gets the edit entity command.
        /// </summary>
        public ICommand EditEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.Edit);
            }
        }

        /// <summary>
        /// Gets the copy entity command.
        /// </summary>
        public ICommand CopyEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.Copy);
            }
        }

        /// <summary>
        /// Gets the delete entity command.
        /// </summary>
        public ICommand DeleteEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.Delete);
            }
        }

        /// <summary>
        /// Gets the command to filter entity columns.
        /// </summary>
        public ICommand FilterEntityColumnCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.FilterColumn);
            }
        }

        /// <summary>
        /// Gets the command to update the visibility of a column.
        /// </summary>
        public ICommand UpdateColumnVisibilityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.UpdateColumnVisibility);
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
        /// Gets the command to load details of a given entity.
        /// </summary>
        public ICommand LoadEntityDetailsCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Entities.LoadDetails);
            }
        }
    }
}