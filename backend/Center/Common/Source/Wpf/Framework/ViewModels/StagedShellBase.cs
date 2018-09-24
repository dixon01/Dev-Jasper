// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StagedShellBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The StagedShellBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    using NLog;

    /// <summary>
    /// Defines a base shell that defines "stages" as main operation areas.
    /// </summary>
    public abstract class StagedShellBase : ShellBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Lazy<ObservableCollection<IStage>> stages;

        private IStage activeStage;

        /// <summary>
        /// Initializes a new instance of the <see cref="StagedShellBase"/> class.
        /// </summary>
        /// <param name="stagedShellBaseParams">The staged shell base parameters.</param>
        protected StagedShellBase(StagedShellBaseParams stagedShellBaseParams)
            : base(
            stagedShellBaseParams.Factory,
            stagedShellBaseParams.HeaderBar,
            stagedShellBaseParams.MenuItems,
            stagedShellBaseParams.StatusBarItems)
        {
            this.stages =
                new Lazy<ObservableCollection<IStage>>(
                    () => this.CreateStagesCollection(stagedShellBaseParams.Stages));
        }

        /// <summary>
        /// Gets or sets the active stage.
        /// </summary>
        /// <value>
        /// The active stage.
        /// </value>
        public IStage ActiveStage
        {
            get
            {
                return this.activeStage;
            }

            set
            {
                if (this.TrySetPropertyBackValue(ref this.activeStage, value))
                {
                    this.RaisePropertyChanged(() => this.ActiveStage);
                }
            }
        }

        /// <summary>
        /// Gets the stages.
        /// </summary>
        /// <value>
        /// The stages.
        /// </value>
        public ObservableCollection<IStage> Stages
        {
            get
            {
                return this.stages.Value;
            }
        }

        /// <summary>
        /// Called when creating stages.
        /// </summary>
        /// <param name="createdStages">The created stages.</param>
        protected virtual void OnCreatingStages(IEnumerable<IStage> createdStages)
        {
        }

        private ObservableCollection<IStage> CreateStagesCollection(
            IEnumerable<Lazy<IStage, IStageMetadata>> lazyStages)
        {
            Logger.Trace("Creating the collection of stages");
            var createdStages = lazyStages.Select(
                stage =>
                    {
                        stage.Value.Index = stage.Metadata.Index;
                        return stage.Value;
                    }).ToList();
            this.OnCreatingStages(createdStages);
            return new ObservableCollection<IStage>(createdStages);
        }

        /// <summary>
        /// Defines the parameters for the constructor of the <see cref="StagedShellBase"/> class.
        /// </summary>
        [Export]
        public class StagedShellBaseParams
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StagedShellBaseParams"/> class.
            /// </summary>
            /// <param name="factory">The factory.</param>
            /// <param name="headerBar">The header bar.</param>
            /// <param name="stages">The stages.</param>
            /// <param name="menuItems">The menu items.</param>
            /// <param name="statusBarItems">The status bar items.</param>
            [ImportingConstructor]
            public StagedShellBaseParams(
                IWindowFactory factory,
                HeaderBarBase headerBar,
                IEnumerable<Lazy<IStage, IStageMetadata>> stages,
                IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> menuItems,
                IEnumerable<Lazy<StatusBarItemBase>> statusBarItems)
            {
                this.Factory = factory;
                this.HeaderBar = headerBar;
                this.Stages = stages;
                this.MenuItems = menuItems;
                this.StatusBarItems = statusBarItems;
            }

            /// <summary>
            /// Gets the factory.
            /// </summary>
            public IWindowFactory Factory { get; private set; }

            /// <summary>
            /// Gets the header bar.
            /// </summary>
            public HeaderBarBase HeaderBar { get; private set; }

            /// <summary>
            /// Gets the stages.
            /// </summary>
            public IEnumerable<Lazy<IStage, IStageMetadata>> Stages { get; private set; }

            /// <summary>
            /// Gets the menu items.
            /// </summary>
            public IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> MenuItems { get; private set; }

            /// <summary>
            /// Gets the status bar items.
            /// </summary>
            public IEnumerable<Lazy<StatusBarItemBase>> StatusBarItems { get; private set; }
        }
    }
}