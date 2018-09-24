// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsistencyMessageDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Consistency
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The consistency message data view model.
    /// </summary>
    public class ConsistencyMessageDataViewModel : DataViewModelBase
    {
        private object source;
        private object sourceParent;
        private Severity severity;
        private string text;
        private string description;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public object Source
        {
            get
            {
                return this.source;
            }

            set
            {
                this.SetProperty(ref this.source, value, () => this.Source);
            }
        }

        /// <summary>
        /// Gets or sets the source parent
        /// </summary>
        public object SourceParent
        {
            get
            {
                return this.sourceParent;
            }

            set
            {
                this.SetProperty(ref this.sourceParent, value, () => this.SourceParent);
            }
        }

        /// <summary>
        /// Gets or sets the text.
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
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Severity"/>.
        /// </summary>
        public Severity Severity
        {
            get
            {
                return this.severity;
            }

            set
            {
                this.SetProperty(ref this.severity, value, () => this.Severity);
            }
        }
    }
}
