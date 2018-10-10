// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// Extension methods for view models.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets the options to display the model on the current main screen.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="state"/> is null.</exception>
        public static void SetMainScreen(this IDisplayViewModel viewModel, IApplicationState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (viewModel.DisplayOptions == null)
            {
                viewModel.DisplayOptions = new DisplayOptions();
            }

            var screenIdentifier = state.DisplayContext == null ? null : state.DisplayContext.MainScreen;
            viewModel.DisplayOptions.Screen = Screen.GetScreen(screenIdentifier);
        }

        /// <summary>
        /// Updates the main screen reference to the one that it is currently displaying the given
        /// <paramref name="window"/>.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="window"/> is null.</exception>
        public static void UpdateMainScreen(this IApplicationState state, Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            state.DisplayContext.MainScreen = ScreenIdentifier.GetFrom(window);
        }
    }
}