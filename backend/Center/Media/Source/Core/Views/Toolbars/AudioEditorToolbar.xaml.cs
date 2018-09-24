// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioEditorToolbar.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AudioEditorToolbar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Toolbars
{
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for AudioEditorToolbar.xaml
    /// </summary>
    public partial class AudioEditorToolbar
    {
        /// <summary>
        /// The dependency property for the currently selected tool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool",
            typeof(EditorToolType),
            typeof(AudioEditorToolbar),
            new PropertyMetadata(EditorToolType.Move));

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEditorToolbar"/> class.
        /// </summary>
        public AudioEditorToolbar()
        {
            InitializeComponent();
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

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var control = (Button)sender;
            if (control.Tag == null)
            {
                return;
            }

            var type = (LayoutElementType)control.Tag;

            var context = (IMediaShell)this.DataContext;
            var editor = context.Editor as AudioEditorViewModel;
            if (editor == null)
            {
                return;
            }

            var insertIndex = editor.CurrentAudioOutputElement.Elements.Count;
            var createParams = new CreateElementParameters { Type = type, InsertIndex = insertIndex };
            editor.CreateLayoutElementCommand.Execute(createParams);
        }
    }
}
