// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGridDefaultDataItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The PropertyGridDefaultDataItem.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System.ComponentModel;

    /// <summary>
    /// The property grid default data item.
    /// </summary>
    /// <typeparam name="T">
    /// the value type
    /// </typeparam>
    public class PropertyGridDefaultDataItem<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// The value.
        /// </summary>
        private T value;

        /// <summary>
        /// The default value.
        /// </summary>
        private T defaultValue;

        /// <summary>
        /// The is default.
        /// </summary>
        private bool isDefault;

        /// <summary>
        /// The tag.
        /// </summary>
        private object tag;

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public T DefaultValue
        {
            get
            {
                return this.defaultValue;
            }

            set
            {
                this.defaultValue = value;
                this.OnPropertyChanged("DefaultValue");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        public bool IsDefault
        {
            get
            {
                return this.isDefault;
            }

            set
            {
                if (this.isDefault != value)
                {
                    this.isDefault = value;
                    this.OnPropertyChanged("IsDefault");
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public object Tag
        {
            get
            {
                return this.tag;
            }

            set
            {
                if (this.tag != value)
                {
                    this.tag = value;
                    this.OnPropertyChanged("Tag");
                }
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
