// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MappingDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The view model for a single mapping in a <see cref="MappingTransformationDataViewModelBase{TConfig}"/>.
    /// </summary>
    public class MappingDataViewModel : INotifyPropertyChanged
    {
        private string from;

        private string to;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the from value.
        /// </summary>
        public string From
        {
            get
            {
                return this.from;
            }

            set
            {
                if (this.from != value)
                {
                    this.from = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the to value.
        /// </summary>
        public string To
        {
            get
            {
                return this.to;
            }

            set
            {
                if (this.to != value)
                {
                    this.to = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}