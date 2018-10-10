// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextReplaceControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ReplaceControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for ReplaceControl.xaml
    /// </summary>
    public partial class TextReplaceControl
    {
        private CollectionViewSource sortedReplacementList;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReplaceControl"/> class.
        /// </summary>
        public TextReplaceControl()
        {
            this.InitializeComponent();

            this.DataContextChanged += this.OnDataContextChanged;
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the validate number function
        /// </summary>
        public Func<string, object, string> ValidateNumber
        {
            get
            {
                return this.DoValidateNumber;
            }
        }

        /// <summary>
        /// Gets the validate input text.
        /// </summary>
        public Func<string, object, string> ValidateInputText
        {
            get
            {
                return this.DoValidateInputText;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as TextReplacementPrompt;
            this.SetReplacementSource(context != null ? context.Replacements : null);
            this.SortReplacementList();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = this.DataContext as TextReplacementPrompt;
            this.SetReplacementSource(context != null ? context.Replacements : null);
        }

        private void OnNumberChanged(string oldNumber, string newNumber, object sourceObject)
        {
            if (!string.IsNullOrEmpty(this.DoValidateNumber(newNumber, sourceObject)))
            {
                return;
            }

            this.TriggerReplacementUpdate(
                sourceObject,
                r => r.Number.Value = Convert.ToInt32(oldNumber),
                r => r.Number.Value = Convert.ToInt32(newNumber));

            this.sortedReplacementList.View.Refresh();
        }

        private void OnDescriptionChanged(string oldDescription, string newDescription, object sourceObject)
        {
            this.TriggerReplacementUpdate(
                sourceObject,
                r => r.Description.Value = oldDescription,
                r => r.Description.Value = newDescription);
        }

        private void OnCodeChanged(string oldCode, string newCode, object sourceObject)
        {
            this.TriggerReplacementUpdate(
                sourceObject,
                r => r.Code.Value = oldCode,
                r => r.Code.Value = newCode);
        }

        private void OnImageChanged(string oldFilename, string newFilename, object sourceObject)
        {
            this.TriggerReplacementUpdate(
                sourceObject,
                r => r.Image.Filename = oldFilename,
                r => r.Image.Filename = newFilename);
        }

        private void TriggerReplacementUpdate(
            object sourceObject,
            Action<TextualReplacementDataViewModel> oldState,
            Action<TextualReplacementDataViewModel> newState)
        {
            var replacement = (TextualReplacementDataViewModel)sourceObject;
            var context = (TextReplacementPrompt)this.DataContext;

            var oldReplacement = (TextualReplacementDataViewModel)replacement.Clone();
            oldState(oldReplacement);

            var newReplacement = (TextualReplacementDataViewModel)replacement.Clone();
            newState(newReplacement);

            var oldReplacements = new List<TextualReplacementDataViewModel> { oldReplacement };
            var newReplacements = new List<TextualReplacementDataViewModel> { newReplacement };
            var parameters = new UpdateEntityParameters(oldReplacements, newReplacements, context.Replacements);

            context.UpdateReplacementCommand.Execute(parameters);
        }

        private string DoValidateNumber(string text, object sourceObject)
        {
            var result = MediaStrings.TextReplaceControl_InvalidNumber;

            int number;
            if (int.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
            {
                if (number <= 99 && number > 0)
                {
                    result = string.Empty;
                }
            }

            return result;
        }

        private string DoValidateInputText(string arg1, object arg2)
        {
            var result = MediaStrings.TextReplaceControl_ContainsSemicolon;

            if (!arg1.Contains(";"))
            {
                result = string.Empty;
            }

            return result;
        }

        private void OnToggleImageAndText(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var viewModel = button.Tag as TextualReplacementDataViewModel;

            if (viewModel != null)
            {
                var isImageReplacement = viewModel.IsImageReplacement;

                this.TriggerReplacementUpdate(
                    viewModel,
                    r => { },
                    r =>
                    {
                        r.IsImageReplacement = !isImageReplacement;

                        if (isImageReplacement)
                        {
                            r.Image = null;
                            r.Code = new DataValue<string>(Settings.Default.CodeConversion_DefaultCode);
                        }
                        else
                        {
                            r.Code = new DataValue<string>(string.Empty);
                        }
                    });
            }
        }

        private void SetReplacementSource(INotifyPropertyChanged container)
        {
            if (container == null)
            {
                if (this.sortedReplacementList != null && this.sortedReplacementList.Source != null)
                {
                    this.sortedReplacementList.Source = null;
                    this.sortedReplacementList.SortDescriptions.Clear();
                }

                return;
            }

            if (this.sortedReplacementList != null && this.sortedReplacementList.Source == container)
            {
                return;
            }

            this.sortedReplacementList = this.TryFindResource("SortedReplacementList") as CollectionViewSource;
            if (this.sortedReplacementList == null)
            {
                return;
            }

            this.sortedReplacementList.Source = container;
            this.sortedReplacementList.SortDescriptions.Add(new SortDescription());

            var context = (TextReplacementPrompt)this.DataContext;
            context.Replacements.CollectionChanged += this.SortReplacementList;
        }

        private void SortReplacementList(
            object obj = null,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs = null)
        {
            if (this.sortedReplacementList != null && this.sortedReplacementList.View != null)
            {
                this.sortedReplacementList.View.Refresh();
            }
        }
    }
}
