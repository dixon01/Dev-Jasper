// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemSettingsStageControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for BackgroundSystemSettingsStageControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Stages
{
    using System.Windows;

    using Gorba.Center.Admin.Core.ViewModels.Stages.Meta;

    /// <summary>
    /// Interaction logic for BackgroundSystemSettingsStageControl.xaml
    /// </summary>
    public partial class BackgroundSystemSettingsStageControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemSettingsStageControl"/> class.
        /// </summary>
        public BackgroundSystemSettingsStageControl()
        {
            this.InitializeComponent();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var context = (BackgroundSystemSettingsViewModel)this.DataContext;
            if (context == null)
            {
                return;
            }

            if (context.UpdateSettingsCommand.CanExecute(context))
            {
                context.UpdateSettingsCommand.Execute(context);
            }
        }
    }
}
