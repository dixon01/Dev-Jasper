// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedEditorToolbar.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LedEditorToolbar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Toolbars
{
    using System.Windows;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Interaction logic for LedEditorToolbar.xaml
    /// </summary>
    public partial class LedEditorToolbar
    {
        /// <summary>
        /// The dependency property for the currently selected tool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool",
            typeof(EditorToolType),
            typeof(LedEditorToolbar),
            new PropertyMetadata(EditorToolType.Move));

        /// <summary>
        /// Initializes a new instance of the <see cref="LedEditorToolbar"/> class.
        /// </summary>
        public LedEditorToolbar()
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
    }
}
