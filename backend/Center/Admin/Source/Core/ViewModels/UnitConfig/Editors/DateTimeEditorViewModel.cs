// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Converters;

    /// <summary>
    /// The view model for a date time editor which can also handle an optional date time.
    /// </summary>
    public class DateTimeEditorViewModel : EditorViewModelBase
    {
        private DateTime? dateTime;

        private bool isNullable;

        private bool hasValue;

        private bool useUtcToUiTimeConverter;

        private DateTime? previousDateTime;

        /// <summary>
        /// Gets or sets the date time value.
        /// </summary>
        public DateTime? DateTime
        {
            get
            {
                return this.dateTime;
            }

            set
            {
                this.SetProperty(ref this.dateTime, value, () => this.DateTime);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor is nullable.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return this.isNullable;
            }

            set
            {
                if (!this.SetProperty(ref this.isNullable, value, () => this.IsNullable))
                {
                    return;
                }

                if (!value)
                {
                    this.HasValue = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user has chosen to enter a value.
        /// This is bound to the checkbox in front of the date time editor.
        /// This property shouldn't be changed if <see cref="IsNullable"/> is set to false.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.hasValue;
            }

            set
            {
                if (!this.SetProperty(ref this.hasValue, value, () => this.HasValue))
                {
                    return;
                }

                if (value)
                {
                    this.DateTime = this.previousDateTime;
                }
                else
                {
                    this.previousDateTime = this.DateTime;
                    this.DateTime = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="UtcToUiTimeConverter"/> should be used
        /// to display the value.
        /// </summary>
        public bool UseUtcToUiTimeConverter
        {
            get
            {
                return this.useUtcToUiTimeConverter;
            }

            set
            {
                this.SetProperty(ref this.useUtcToUiTimeConverter, value, () => this.UseUtcToUiTimeConverter);
            }
        }
    }
}
