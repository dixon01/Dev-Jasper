// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssTickerComposer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System.ComponentModel;
    using System.Text;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Special composer handling <see cref="RssTickerElementDataViewModel"/>.
    /// </summary>
    public class RssTickerComposer : DrawableComposerBase<RssTickerElementDataViewModel, TextItem>
    {
        private FontDataViewModel currentFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssTickerComposer"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        public RssTickerComposer(IComposerContext context, IComposer parent, RssTickerElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Align = this.ViewModel.Align.Value;
            this.Item.VAlign = this.ViewModel.VAlign.Value;
            this.Item.ScrollSpeed = this.ViewModel.ScrollSpeed.Value;
            this.Item.Text = this.ViewModel.TestData.Value;
            this.Item.Overflow = TextOverflow.ScrollRing;
            this.currentFont = this.ViewModel.Font;
            this.currentFont.PropertyChanged += this.FontOnPropertyChanged;

            this.UpdateFont();
            this.UpdateText();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            this.currentFont.PropertyChanged -= this.FontOnPropertyChanged;
            base.Dispose();
        }

        /// <summary>
        /// Method to be overridden by subclasses to handle the change of a property of the data view model.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected override void HandlePropertyChange(string propertyName)
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
                case "TestData":
                case "Delimiter":
                    this.UpdateText();
                    break;
                case "Align":
                    this.Item.Align = this.ViewModel.Align.Value;
                    break;
                case "VAlign":
                    this.Item.VAlign = this.ViewModel.VAlign.Value;
                    break;
                case "ScrollSpeed":
                    this.Item.ScrollSpeed = this.ViewModel.ScrollSpeed.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }

        private void UpdateFont()
        {
            this.Item.Font = this.currentFont.Export();
        }

        private void UpdateText()
        {
            if (this.ViewModel.TestData == null || string.IsNullOrEmpty(this.ViewModel.TestData.Value))
            {
                this.Item.Text = string.Empty;
                return;
            }

            var testTexts = this.ViewModel.TestData.Value.Split(';');
            var text = new StringBuilder();
            foreach (var testText in testTexts)
            {
                text.Append(testText);
                text.Append(this.ViewModel.Delimiter.Value);
            }

            this.Item.Text = text.ToString();
        }

        private void FontOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateFont();
        }
    }
}
