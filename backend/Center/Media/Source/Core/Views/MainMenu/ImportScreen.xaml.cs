// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportScreen.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ImportScreen.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// Interaction logic for ImportScreen.xaml
    /// </summary>
    public partial class ImportScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportScreen"/> class.
        /// </summary>
        public ImportScreen()
        {
            InitializeComponent();
        }

        private void OnKeyUpHandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void BrowseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Action<OpenFileDialogInteraction> onOpen = interaction =>
            {
                if (!interaction.Confirmed)
                {
                    return;
                }

                var context = (MainMenuPrompt)this.DataContext;
                context.ImportFilePath = interaction.FileName;
                if (string.IsNullOrEmpty(context.NewProjectName))
                {
                    context.NewProjectName = Path.GetFileNameWithoutExtension(interaction.FileName);
                }
            };

            var openDialogInteraction = new OpenFileDialogInteraction
            {
                Filter = MediaStrings.OpenFileDialog_ProjectFilter,
                Title = MediaStrings.ImportMenu_ImportPathTitle,
                DirectoryType = DialogDirectoryTypes.Project,
                MultiSelect = false
            };
            InteractionManager<OpenFileDialogInteraction>.Current.Raise(openDialogInteraction, onOpen);
        }
    }
}
