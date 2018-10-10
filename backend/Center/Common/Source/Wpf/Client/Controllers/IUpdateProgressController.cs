// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateProgressController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateProgressController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The application update progress controller interface.
    /// </summary>
    public interface IUpdateProgressController : IDialogController
    {
        /// <summary>
        /// Gets the update progress view model of this controller.
        /// </summary>
        UpdateProgressViewModel UpdateProgress { get; }
    }
}