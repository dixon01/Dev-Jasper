// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminShell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdminShell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels.Editor;
    using Gorba.Center.Admin.Core.ViewModels.Navigator;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    using NLog;

    /// <summary>
    /// The icenter.admin shell.
    /// </summary>
    [Export(typeof(IAdminShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class AdminShell : ClientShellBase, IAdminShell, IWeakEventListener
    {
        /// <summary>
        /// The default window title.
        /// </summary>
        public static readonly string DefaultWindowTitle = "icenter.admin";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly Lazy<ICollectionView> statusNotifications;

        private StageViewModelBase currentStage;

        private bool isBusy;

        private string busyMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminShell"/> class.
        /// </summary>
        /// <param name="factory">The shell factory.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public AdminShell(AdminShellFactory factory, ICommandRegistry commandRegistry)
            : base(factory, null, null, null, commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Title = DefaultWindowTitle;

            this.Notifications = new ObservableCollection<Notification>();
            this.OverallProgress = new ProgressNotification();

            Predicate<object> filterStatusNotifications = o =>
                {
                    var notification = o as StatusNotification;
                    if (notification == null)
                    {
                        return false;
                    }

                    return !notification.IsAcknowledged;
                };
            this.statusNotifications = new Lazy<ICollectionView>(
                () =>
                {
                    var source = CollectionViewSource.GetDefaultView(this.Notifications);
                    source.Filter = filterStatusNotifications;
                    return source;
                });

            this.Navigator = new NavigatorViewModel(commandRegistry);
            this.Navigator.PropertyChanged += this.NavigatorOnPropertyChanged;

            this.EntityStages = new ObservableCollection<EntityStageViewModelBase>();
            this.RemovableMediaStages = new ObservableCollection<RemovableMediaStageViewModel>();

            this.HomeStage = new HomeStageViewModel(this.commandRegistry);
            this.CurrentStage = this.HomeStage;

            this.Editor = new EntityEditorViewModel(this.commandRegistry);
        }

        /// <summary>
        /// Gets the notifications
        /// </summary>
        public ObservableCollection<Notification> Notifications { get; private set; }

        /// <summary>
        /// Gets the status notifications
        /// </summary>
        public ICollectionView StatusNotifications
        {
            get
            {
                return this.statusNotifications.Value;
            }
        }

        /// <summary>
        /// Gets or set the overall progress
        /// </summary>
        public ProgressNotification OverallProgress { get; private set; }

        /// <summary>
        /// Gets the navigator view model.
        /// </summary>
        public NavigatorViewModel Navigator { get; private set; }

        /// <summary>
        /// Gets the home stage view model.
        /// </summary>
        public HomeStageViewModel HomeStage { get; private set; }

        /// <summary>
        /// Gets the list of all entity stage view models.
        /// </summary>
        public ObservableCollection<EntityStageViewModelBase> EntityStages { get; private set; }

        /// <summary>
        /// Gets the list of all stages for removable media (USB sticks).
        /// </summary>
        public ObservableCollection<RemovableMediaStageViewModel> RemovableMediaStages { get; private set; }

        /// <summary>
        /// Gets or sets the currently displayed stage view model.
        /// </summary>
        public StageViewModelBase CurrentStage
        {
            get
            {
                return this.currentStage;
            }

            set
            {
                if (!this.SetProperty(ref this.currentStage, value, () => this.CurrentStage))
                {
                    return;
                }

                var entity = value as EntityStageViewModelBase;
                if (entity != null)
                {
                    this.Navigator.SelectedEntity =
                        this.Navigator.Partitions.SelectMany(p => p.Entities).FirstOrDefault(
                            e => e.Name == entity.Name);

                    this.Navigator.HomeIsSelected = false;
                    this.Navigator.SelectedRemovableMedia = null;
                    return;
                }

                var removable = value as RemovableMediaStageViewModel;
                if (removable != null)
                {
                    this.Navigator.SelectedEntity = null;
                    this.Navigator.HomeIsSelected = false;
                    this.Navigator.SelectedRemovableMedia =
                        this.Navigator.RemovableMedia.FirstOrDefault(m => m.Name == removable.Name);
                    return;
                }

                this.Navigator.SelectedEntity = null;
                this.Navigator.HomeIsSelected = true;
                this.Navigator.SelectedRemovableMedia = null;
            }
        }

        /// <summary>
        /// Gets the entity editor.
        /// </summary>
        public EntityEditorViewModel Editor { get; private set; }

        /// <summary>
        /// Gets the application state
        /// </summary>
        public IAdminApplicationState AdminApplicationState
        {
            get
            {
                return this.ApplicationState as IAdminApplicationState;
            }
        }

        /// <summary>
        /// Gets the show file menu command.
        /// </summary>
        public ICommand ShowFileMenuCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowFileMenu);
            }
        }

        /// <summary>
        /// Gets the file menu interaction request.
        /// </summary>
        public IInteractionRequest FileMenuInteractionRequest
        {
            get
            {
                return InteractionManager<FileMenuPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the about screen interaction request.
        /// </summary>
        public IInteractionRequest AboutScreenRequest
        {
            get
            {
                return InteractionManager<AboutScreenPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the option dialog interaction request.
        /// </summary>
        public IInteractionRequest OptionDialogRequest
        {
            get
            {
                return InteractionManager<OptionsPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the complete window is busy. This is used to disable the window
        /// while a modal dialog is open.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, () => this.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        public string BusyMessage
        {
            get
            {
                return this.busyMessage;
            }

            set
            {
                this.SetProperty(ref this.busyMessage, value, () => this.BusyMessage);
            }
        }

        /// <summary>
        /// Handling of weak events
        /// </summary>
        /// <param name="managerType">the type of the manager</param>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event arguments</param>
        /// <returns>a boolean indicating if the event was successfully handled</returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = (PropertyChangedEventArgs)e;
            return this.OnPropertyChanged(args);
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        protected override void CreateWindow()
        {
            Logger.Trace("Creating window");
            base.CreateWindow();
            PropertyChangedEventManager.AddListener(this.AdminApplicationState, this, "CurrentTenant");
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            return true;
        }

        private void NavigatorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedEntity")
            {
                if (this.Navigator.SelectedEntity == null)
                {
                    return;
                }

                var stage = this.EntityStages.FirstOrDefault(s => s.Name == this.Navigator.SelectedEntity.Name);
                if (stage != null)
                {
                    this.CurrentStage = stage;
                }

                return;
            }

            if (e.PropertyName == "SelectedRemovableMedia")
            {
                if (this.Navigator.SelectedRemovableMedia == null)
                {
                    return;
                }

                var stage =
                    this.RemovableMediaStages.FirstOrDefault(s => s.Name == this.Navigator.SelectedRemovableMedia.Name);
                if (stage != null)
                {
                    this.CurrentStage = stage;
                }
            }
        }
    }
}