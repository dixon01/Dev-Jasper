// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientShellBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientShellBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The base class for application shells of client applications.
    /// </summary>
    public abstract class ClientShellBase : ShellBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientShellBase"/> class.
        /// </summary>
        /// <param name="factory">The window factory.</param>
        /// <param name="headerBar">The header bar.</param>
        /// <param name="menuItems">The top menu items.</param>
        /// <param name="statusBarItems">The status bar Items.</param>
        /// <param name="commandRegistry">The command registry.</param>
        protected ClientShellBase(
            IWindowFactory factory,
            HeaderBarBase headerBar,
            IEnumerable<Lazy<MenuItemBase, IMenuItemMetadata>> menuItems,
            IEnumerable<Lazy<StatusBarItemBase>> statusBarItems,
            ICommandRegistry commandRegistry)
            : base(factory, headerBar, menuItems, statusBarItems)
        {
            this.LoginInformation = new LoginInformationViewModel(commandRegistry);
        }

        /// <summary>
        /// Gets the view model representing the login information shown in the upper right corner.
        /// </summary>
        public LoginInformationViewModel LoginInformation { get; private set; }

        /// <summary>
        /// Gets the connection exception interaction request.
        /// </summary>
        public IInteractionRequest ConnectionExceptionInteractionRequest
        {
            get
            {
                return InteractionManager<ConnectionExceptionPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the interaction request to change the current user's password.
        /// </summary>
        public IInteractionRequest ChangePasswordInteractionRequest
        {
            get
            {
                return InteractionManager<ChangePasswordPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }
    }
}
