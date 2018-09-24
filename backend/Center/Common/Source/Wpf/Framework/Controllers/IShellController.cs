// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Defines a shell controller.
    /// </summary>
    public interface IShellController
    {
        /// <summary>
        /// Gets the shell.
        /// </summary>
        IShellViewModel Shell { get; }

        /// <summary>
        /// Gets the options controller.
        /// </summary>
        OptionsController OptionsController { get; }
    }
}
