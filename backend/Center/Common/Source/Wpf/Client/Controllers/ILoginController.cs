// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoginController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Login controller for Center applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Login controller for Center applications.
    /// </summary>
    public interface ILoginController : IDialogController
    {
        /// <summary>
        /// Gets the login window.
        /// </summary>
        ILoginViewModel Login { get; }
    }
}
