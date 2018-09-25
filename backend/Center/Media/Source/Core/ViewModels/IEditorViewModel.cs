// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IEditorViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The Editor ViewModel interface
    /// </summary>
    public interface IEditorViewModel
    {
        /// <summary>
        /// Gets or sets the parent view model.
        /// </summary>
        MediaShell Parent { get; set; }

        /// <summary>
        /// Gets the snap configuration for the current editor.
        /// </summary>
        SnapConfiguration SnapConfiguration { get; }

        /// <summary>
        /// Gets the paste configuration for the current editor.
        /// </summary>
        PasteConfiguration PasteConfiguration { get; }

        /// <summary>
        /// Gets or sets the current selected element
        /// </summary>
        ExtendedObservableCollection<LayoutElementDataViewModelBase> SelectedElements { get; set; }

        /// <summary>
        /// Gets or sets the layout elements.
        /// </summary>
        ExtendedObservableCollection<GraphicalElementDataViewModelBase> Elements { get; set; }

        /// <summary>
        /// Gets or sets the current audio output element.
        /// </summary>
        AudioOutputElementDataViewModel CurrentAudioOutputElement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can create audio elements.
        /// </summary>
        bool CanCreateAudioElements { get; set; }

        /// <summary>
        /// Gets the create layout element command.
        /// </summary>
        ICommand CreateLayoutElementCommand { get; }

        /// <summary>
        /// Takes a screenshot of the current selected Layout
        /// </summary>
        /// <param name="encoder">
        /// The <see cref="BitmapEncoder"/>.
        /// </param>
        void TakeScreenshot(BitmapEncoder encoder);

        /// <summary>
        /// The sort by z order.
        /// </summary>
        void SortByZOrder();
    }
}