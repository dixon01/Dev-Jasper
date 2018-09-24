// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckInDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Interaction logic for CheckInDialog.xaml
    /// </summary>
    public partial class CheckInDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckInDialog"/> class.
        /// </summary>
        public CheckInDialog()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) => this.MinorVersionRadioButton.IsChecked = true;
            this.Loaded += (sender, args) => this.Description.Focus();
        }

        /// <summary>
        /// Gets the create cycle command wrapper.
        /// </summary>
        public ICommand CheckInCommandWrapper
        {
            get
            {
                return new RelayCommand(this.OnCheckIn, this.CanCheckIn);
            }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(this.Cancel);
            }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand SkipCommand
        {
            get
            {
                return new RelayCommand(this.Skip);
            }
        }

        private bool CanCheckIn(object obj)
        {
            var context = (CheckInPrompt)this.DataContext;
            if (context == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.Description.Text))
            {
                return false;
            }

            return context.ConnectedApplicationState.CurrentTenant == null
                    || (context.PermissionController.HasPermission(Permission.Write, context.RequiredDataScope)
                        && ((this.MinorVersionRadioButton.IsChecked.HasValue
                             && this.MinorVersionRadioButton.IsChecked.Value)
                            || (this.MajorVersionRadioButton.IsChecked.HasValue
                                && this.MajorVersionRadioButton.IsChecked.Value)));
        }

        private void OnCheckIn()
        {
            var context = (CheckInPrompt)this.DataContext;
            if (context == null)
            {
                this.Close();
                return;
            }

            var selectedVersionType = (this.MinorVersionRadioButton.IsChecked.HasValue
                                        && this.MinorVersionRadioButton.IsChecked.Value)
                ? context.Minor
                : context.Major;
            var pair = selectedVersionType.Split('.');
            var major = int.Parse(pair[0]);
            var minor = int.Parse(pair[1]);

            context.OnCheckinCompleted(new CheckinTrapResult
            {
                Major = major,
                Minor = minor,
                CheckinComment = context.CheckInComment,
                Decision = CheckinUserDecision.Checkin
            });

            this.Close();
        }

        private void Cancel()
        {
            var context = (CheckInPrompt)this.DataContext;
            if (context != null)
            {
                context.OnCheckinCompleted(new CheckinTrapResult
                                                {
                                                    Decision = CheckinUserDecision.Cancel
                                                });
            }

            this.Close();
        }

        private void Skip()
        {
            var context = (CheckInPrompt)this.DataContext;
            if (context != null)
            {
                context.OnCheckinCompleted(new CheckinTrapResult
                                                    {
                                                        Decision = CheckinUserDecision.Skip
                                                    });
            }

            this.Close();
        }

        private void CheckInCommentsOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
