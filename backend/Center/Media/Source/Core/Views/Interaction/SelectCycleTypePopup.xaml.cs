// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectCycleTypePopup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CreateCyclePopup.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;

    using NLog;

    /// <summary>
    /// Interaction logic for CreateCyclePopup.xaml
    /// </summary>
    public partial class SelectCycleTypePopup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCycleTypePopup"/> class.
        /// </summary>
        public SelectCycleTypePopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the create cycle command wrapper.
        /// </summary>
        public ICommand CreateCycleCommandWrapper
        {
            get
            {
                return new RelayCommand(this.OnCreateCycle, this.CanCreateCycle);
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

        private void Cancel()
        {
            this.Close();
        }

        private bool CanCreateCycle(object o)
        {
            return this.CycleTypeCombobox.SelectedItem != null;
        }

        private void OnCreateCycle()
        {
            if (this.CycleTypeCombobox.SelectedItem == null)
            {
                return;
            }

            var selectedEnumerationMember =
                this.CycleTypeCombobox.SelectedItem as EnumerationExtension.EnumerationMember;

            if (selectedEnumerationMember == null)
            {
                Logger.Error("Unknown combobox value selected.");
                return;
            }

            if (!(this.DataContext is SelectCycleTypePrompt prompt))
            {
                return;
            }

            prompt.CreateCycleCommand.Execute(selectedEnumerationMember.Value);
            this.Close();
        }
    }
}
