// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Editors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Interaction logic for CsvEditor.xaml
    /// </summary>
    public partial class CsvEditor
    {
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvEditor"/> class.
        /// </summary>
        public CsvEditor()
        {
            this.InitializeComponent();

            this.isLoading = true;
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(this.Close);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoading = false;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.isLoading)
            {
                return;
            }

            var context = (CsvEditorViewModel)this.DataContext;
            context.IsDirty = true;
        }
    }
}
