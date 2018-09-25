// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DialogFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Factory to create dialogs.
    /// </summary>
    /// <typeparam name="T">The type of the dialog to create.</typeparam>
    public class DialogFactory<T> : IDialogFactory
        where T : IDialogView
    {
        /// <summary>
        /// Creates a window binding its context to the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>
        /// A new window with the given ViewModel as DataContext.
        /// </returns>
        public virtual IDialogView Create(IDialogViewModel viewModel)
        {
            try
            {
                var dialog = Activator.CreateInstance<T>();
                dialog.DataContext = viewModel;
                return dialog;
            }
            catch (Exception exception)
            {
                throw new ApplicationException(
                    string.Format(
                        "Error while creating a new dialog of type {0}."
                        + " This default implementation only works if a default constructor is defined.",
                        typeof(T).FullName),
                    exception);
            }
        }
    }
}
