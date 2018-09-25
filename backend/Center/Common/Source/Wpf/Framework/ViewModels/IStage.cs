// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Defines a stage.
    /// </summary>
    public interface IStage : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        IStageController Controller { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the navigation bar image.
        /// </summary>
        BitmapImage NavBarImage { get; set; }

        /// <summary>
        /// Gets the stage authorization context.
        /// </summary>
        IStageAuthorizationContext StageAuthorizationContext { get; }

        /// <summary>
        /// Updates the specified stage authorization context.
        /// </summary>
        /// <param name="authorizationContext">The stage authorization context.</param>
        void UpdateAuthorizationContext(IStageAuthorizationContext authorizationContext);
    }
}