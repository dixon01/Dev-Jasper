// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisplayViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDisplayViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines a view model that can be displayed.
    /// </summary>
    public interface IDisplayViewModel : IViewModel
    {
        /// <summary>
        /// Gets or sets the display options for this model.
        /// </summary>
        DisplayOptions DisplayOptions { get; set; }
    }
}