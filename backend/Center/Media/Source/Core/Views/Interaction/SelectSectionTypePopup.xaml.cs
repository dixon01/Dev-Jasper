// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectSectionTypePopup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SelectSectionTypePopup.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// Interaction logic for SelectSectionTypePopup.xaml
    /// </summary>
    public partial class SelectSectionTypePopup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSectionTypePopup"/> class.
        /// </summary>
        public SelectSectionTypePopup()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the create section command wrapper.
        /// </summary>
        public ICommand CreateSectionCommandWrapper
        {
            get
            {
                return new RelayCommand(this.OnCreateSection, this.CanCreateSection);
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

        private bool CanCreateSection(object o)
        {
            return this.SectionTypeCombobox.SelectedItem != null && this.LayoutCombobox.SelectedItem != null;
        }

        private void OnCreateSection()
        {
            if (this.SectionTypeCombobox.SelectedItem == null)
            {
                return;
            }

            var selectedEnumerationMember =
                this.SectionTypeCombobox.SelectedItem as EnumerationExtension.EnumerationMember;

            if (selectedEnumerationMember == null)
            {
                Logger.Error("Unknown combobox value selected.");
                return;
            }

            if (!(this.DataContext is SelectSectionTypePrompt prompt))
            {
                return;
            }

            var parameters = new CreateSectionParameters
                                 {
                                     Layout = (LayoutConfigDataViewModel)this.LayoutCombobox.SelectedItem,
                                     SectionType = (SectionType)selectedEnumerationMember.Value
                                 };
            prompt.CreateSectionCommand.Execute(parameters);
            this.Close();
        }

        private void ComboboxOnDropDownOpened(object sender, EventArgs e)
        {
            var selectSectionTypePrompt = this.DataContext as SelectSectionTypePrompt;
            if (selectSectionTypePrompt != null)
            {
                selectSectionTypePrompt.SuppressMouseEvents = true;
            }
        }

        private void ComboboxOnDropDownClosed(object sender, EventArgs e)
        {
            var selectSectionTypePrompt = this.DataContext as SelectSectionTypePrompt;
            if (selectSectionTypePrompt != null)
            {
                selectSectionTypePrompt.SuppressMouseEvents = false;
            }
        }
    }
}
