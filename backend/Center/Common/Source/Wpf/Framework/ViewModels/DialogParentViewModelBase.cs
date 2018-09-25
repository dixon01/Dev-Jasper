// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogParentViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the base for parent windows.
    /// </summary>
    public abstract class DialogParentViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the display options for this model.
        /// </summary>
        public DisplayOptions DisplayOptions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Window" />.
        /// </summary>
        internal Window Window { get; set; }
    }
}