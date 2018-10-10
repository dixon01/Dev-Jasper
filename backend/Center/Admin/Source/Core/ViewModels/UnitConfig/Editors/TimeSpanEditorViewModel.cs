// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSpanEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;

    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// The time span editor view model.
    /// </summary>
    public class TimeSpanEditorViewModel : EditorViewModelBase
    {
        private TimeSpan? value;

        private bool isNullable;

        private string emptyContent = AdminStrings.Editor_EnterDuration;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public TimeSpan? Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (!value.HasValue && !this.IsNullable)
                {
                    throw new ArgumentNullException("value", "Editor is not set to be nullable");
                }

                if (value.HasValue)
                {
                    if (value.Value.TotalDays >= 1)
                    {
                        throw new ArgumentOutOfRangeException("value", "Only values up to 23:59:59.999 are supported");
                    }

                    if (value.Value.TotalDays < 0)
                    {
                        throw new ArgumentOutOfRangeException("value", "Only positive values and zero are supported");
                    }
                }

                if (this.SetProperty(ref this.value, value, () => this.Value))
                {
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this editor supports null values.
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return this.isNullable;
            }

            set
            {
                if (this.SetProperty(ref this.isNullable, value, () => this.IsNullable))
                {
                    this.EmptyContent = value
                                            ? AdminStrings.Editor_EnterDurationOrEmpty
                                            : AdminStrings.Editor_EnterDuration;
                }
            }
        }

        /// <summary>
        /// Gets the empty content string shown when no value is entered in the editor.
        /// </summary>
        public string EmptyContent
        {
            get
            {
                return this.emptyContent;
            }

            private set
            {
                this.SetProperty(ref this.emptyContent, value, () => this.EmptyContent);
            }
        }
    }
}
