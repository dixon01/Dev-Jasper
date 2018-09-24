// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerEditorDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TriggerEditorDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// Interaction logic for EditMenuDialog.xaml
    /// </summary>
    public partial class TriggerEditorDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerEditorDialog"/> class.
        /// </summary>
        public TriggerEditorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the add coordinate wrapper
        /// </summary>
        public ICommand AddCoordinateWrapper
        {
            get
            {
                return new RelayCommand(this.OnAddCoordinate);
            }
        }

        /// <summary>
        /// Gets the Remove coordinate command
        /// </summary>
        public ICommand RemoveCoordinate
        {
            get
            {
                return new RelayCommand(this.OnRemoveCoordinate);
            }
        }

        /// <summary>
        /// Gets the show dictionary selector
        /// </summary>
        public ICommand ShowDictionarySelector
        {
            get
            {
                return new RelayCommand(this.OnShowDictionarySelector);
            }
        }

        private void OnAddCoordinate()
        {
            if (!(this.DataContext is TriggerNavigationEditorPrompt context))
            {
                return;
            }

            var newCoordinate = new GenericEvalDataViewModel(context.Shell);
            context.DataValue.Coordinates.Add(newCoordinate);
        }

        private void OnRemoveCoordinate(object obj)
        {
            var coordinate = (GenericEvalDataViewModel)obj;

            if (!(this.DataContext is TriggerNavigationEditorPrompt context))
            {
                return;
            }

            if (context.DataValue.Coordinates.Count == 1)
            {
                MessageBox.Show(
                    MediaStrings.TriggerEditor_CantRemoveLastCoordinateError,
                    MediaStrings.TriggerEditor_CantRemoveLastCoordinateErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                context.DataValue.Coordinates.Remove(coordinate);
            }
        }

        private void OnShowDictionarySelector(object obj)
        {
            var coordinate = (GenericEvalDataViewModel)obj;

            if (!(this.DataContext is TriggerNavigationEditorPrompt context))
            {
                return;
            }

            context.ShowDictionarySelectorCommand.Execute(coordinate);
        }
    }
}
