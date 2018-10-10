// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextComposer.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System.ComponentModel;
    using System.Text;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Composer handling <see cref="TextElementDataViewModel"/>.
    /// </summary>
    public partial class TextComposer
    {
        private FontDataViewModel currentFont;

        partial void Initialize()
        {
            this.currentFont = this.ViewModel.Font;
            this.currentFont.PropertyChanged += this.FontOnPropertyChanged;

            this.UpdateFont();
            this.UpdateText();
        }

        partial void Deinitialize()
        {
            this.currentFont.PropertyChanged -= this.FontOnPropertyChanged;
        }

        partial void PreHandlePropertyChange(string propertyName, ref bool handled)
        {
            switch (propertyName)
            {
                case "Font":
                    this.currentFont.PropertyChanged -= this.FontOnPropertyChanged;
                    this.currentFont = this.ViewModel.Font;
                    this.currentFont.PropertyChanged += this.FontOnPropertyChanged;
                    this.UpdateFont();
                    break;
                case "Value":
                case "SelectedDictionaryValue":
                case "TestData":
                    this.UpdateText();
                    break;
                default:
                    return;
            }

            handled = true;
        }

        private void UpdateFont()
        {
            this.Item.Font = this.currentFont.Export();
        }

        private void UpdateText()
        {
            var viewModel = this.ViewModel as DynamicTextElementDataViewModel;
            if (viewModel == null)
            {
                this.Item.Text = this.ViewModel.Value.Value;
                return;
            }

            if (viewModel.TestData != null && !string.IsNullOrEmpty(viewModel.TestData.Value))
            {
                this.Item.Text = viewModel.TestData.Value;
                return;
            }

            // create a test text from the selected dictionary value
            var generic = viewModel.SelectedDictionaryValue;
            var text = new StringBuilder();
            text.AppendFormat("${0}.{1}", generic.Table.Name, generic.Column.Name);

            if (generic.Table.MultiRow)
            {
                text.AppendFormat("[{0}]", generic.Row);
            }

            if (generic.Table.MultiLanguage)
            {
                text.AppendFormat("{{{0}}}", generic.Language.Name);
            }

            this.Item.Text = text.ToString();
        }

        private void FontOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateFont();
        }
    }
}
