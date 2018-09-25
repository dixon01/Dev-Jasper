// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftEditorToolbar.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Views.Toolbars
{
    using System;
    using System.Windows;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Interaction logic for Toolbar.xaml
    /// </summary>
    public partial class TftEditorToolbar
    {
        /// <summary>
        /// The dependency property for the currently selected tool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool",
            typeof(EditorToolType),
            typeof(TftEditorToolbar),
            new PropertyMetadata(EditorToolType.Move));

        /// <summary>
        /// Initializes a new instance of the <see cref="TftEditorToolbar" /> class.
        /// </summary>
        public TftEditorToolbar()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the currently selected tool
        /// </summary>
        public EditorToolType SelectedTool
        {
            get
            {
                return (EditorToolType)this.GetValue(SelectedToolProperty);
            }

            set
            {
                this.SetValue(SelectedToolProperty, value);
            }
        }

        private void OnLiveInformationToolSelected(object sender, RoutedEventArgs e)
        {
            this.LiveInformationToolParent.IsOpen = false;
            var context = this.DataContext as TftEditorToolbarViewModel;

            if (context == null)
            {
                throw new Exception("No view model for toolbar.");
            }

            // ensure already selected button is setting value
            context.SetSelectedEditorTool();
        }

        private void OnLiveInformationToolParentClicked(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as TftEditorToolbarViewModel;

            if (context == null)
            {
                throw new Exception("No view model for toolbar.");
            }

            // ensure clicking on parent button will select the current tool
            context.SetSelectedEditorTool();
        }
    }
}
