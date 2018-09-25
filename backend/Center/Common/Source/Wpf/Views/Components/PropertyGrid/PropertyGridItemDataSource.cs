// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridItemDataSource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Validation;

    /// <summary>
    /// The property grid item data source.
    /// </summary>
    public class PropertyGridItemDataSource : ValidationViewModelBase, IDataErrorInfo
    {
        /// <summary>
        /// The item.
        /// </summary>
        private readonly PropertyGridItem item;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridItemDataSource"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public PropertyGridItemDataSource(PropertyGridItem item)
        {
            this.item = item;
            this.Error = string.Empty;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get
            {
                if (this.item.DataSource is TimeSpan)
                {
                    if (!string.IsNullOrEmpty(this.item.FormatString))
                    {
                        return ((TimeSpan)this.item.DataSource).ToString(this.item.FormatString);
                    }

                    return ((TimeSpan)this.item.DataSource).ToString(@"hh\:mm\:ss\.f");
                }

                return this.item.DataSource;
            }

            set
            {
                if (this.item.DataSource != value)
                {
                    if (this.item.DataSource is TimeSpan)
                    {
                        try
                        {
                            this.item.DataSource = TimeSpan.Parse(value as string);
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                    else
                    {
                        this.item.DataSource = value;
                    }

                    this.RaisePropertyChanged(() => this.Value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the parallel value.
        /// </summary>
        public object ParallelValue
        {
            get
            {
                return this.item.ParallelData;
            }

            set
            {
                if (this.item.ParallelData != value)
                {
                    this.item.ParallelData = value;
                    this.RaisePropertyChanged(() => this.ParallelValue);
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object DomainObject
        {
            get
            {
                return this.item.DomainObject;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is multi select.
        /// </summary>
        public bool IsMultiSelect
        {
            get
            {
                return this.item.IsMultiSelect;
            }
        }

        /// <summary>
        /// Gets the navigate button command. It executes the NavigateButtonAction of <see cref="PropertyGridItem"/>.
        /// </summary>
        public ICommand NavigateButtonCommand
        {
            get
            {
                return this.item.NavigateButtonAction != null ? new RelayCommand(this.item.NavigateButtonAction) : null;
            }
        }

        /// <summary>
        /// Gets the navigate button text.
        /// </summary>
        public string NavigateButtonText
        {
            get
            {
                return this.item.NavigateButtonText;
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                if (columnName == "Value")
                {
                    var parent = this.item.ParentObject as ValidationViewModelBase;
                    if (parent != null)
                    {
                        return parent.IsValid(this.item.Name, this.Value);
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// The invalidate.
        /// </summary>
        public void Invalidate()
        {
            this.RaisePropertyChanged(() => this.Value);
        }
    }
}
