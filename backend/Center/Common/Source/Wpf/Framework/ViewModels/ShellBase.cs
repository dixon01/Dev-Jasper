// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ShellBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the base class for all shells.
    /// A shell is the main window for an application.
    /// </summary>
    public abstract class ShellBase : WindowViewModelCloseStrategyBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HeaderBarBase headerBar;

        private readonly Lazy<ObservableCollection<MenuItemBase>> menuCollection;

        private readonly Lazy<ObservableCollection<StatusBarItemBase>> statusBarCollection;

        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellBase"/> class.
        /// </summary>
        /// <param name="factory">The window factory.</param>
        /// <param name="headerBar">The header bar.</param>
        /// <param name="menuItems">The top menu items.</param>
        /// <param name="statusBarItems">The status bar Items.</param>
        protected ShellBase(
            IWindowFactory factory,
            HeaderBarBase headerBar,
            IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> menuItems,
            IEnumerable<Lazy<StatusBarItemBase>> statusBarItems)
            : base(factory)
        {
            this.headerBar = headerBar;
            this.menuCollection =
                new Lazy<ObservableCollection<MenuItemBase>>(() => this.CreateMenuCollection(menuItems));
            this.statusBarCollection =
                new Lazy<ObservableCollection<StatusBarItemBase>>(() => this.CreateStatusBarCollection(statusBarItems));
        }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        public IApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationState>();
            }
        }

        /// <summary>
        /// Gets the top menu items.
        /// </summary>
        public ObservableCollection<MenuItemBase> MenuItems
        {
            get
            {
                return this.menuCollection.Value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the application.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.SetProperty(ref this.title, value, () => this.Title);
            }
        }

        /// <summary>
        /// Gets the header bar.
        /// </summary>
        public HeaderBarBase HeaderBar
        {
            get
            {
                return this.headerBar;
            }
        }

        /// <summary>
        /// Gets the status bar items.
        /// </summary>
        public ObservableCollection<StatusBarItemBase> StatusBarItems
        {
            get
            {
                return this.statusBarCollection.Value;
            }
        }

        private ObservableCollection<MenuItemBase> CreateMenuCollection(
            IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> lazyMenuItems)
        {
            Logger.Trace("Creating the top menu");
            return
                new ObservableCollection<MenuItemBase>(
                    lazyMenuItems.Select(
                        menu =>
                        {
                            menu.Value.Index = menu.Metadata.Index;
                            return menu.Value;
                        }));
        }

        private ObservableCollection<StatusBarItemBase> CreateStatusBarCollection(
            IEnumerable<Lazy<StatusBarItemBase>> lazyStatusBarItems)
        {
            Logger.Trace("Creating the status bar");
            return
                new ObservableCollection<StatusBarItemBase>(
                    lazyStatusBarItems.Select(
                    statusBarItem => statusBarItem.Value));
        }
    }
}