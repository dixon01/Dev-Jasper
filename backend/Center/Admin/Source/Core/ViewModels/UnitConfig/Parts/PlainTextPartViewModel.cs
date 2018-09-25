// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlainTextPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlainTextPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple part of the  unit configurator navigation tree that shows a single text.
    /// </summary>
    public class PlainTextPartViewModel : PartViewModelBase
    {
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextPartViewModel"/> class.
        /// </summary>
        public PlainTextPartViewModel()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.SetProperty(ref this.text, value, () => this.Text);
            }
        }

        /// <summary>
        /// Gets all errors of this node (always empty collection).
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return new ErrorItem[0];
            }
        }
    }
}