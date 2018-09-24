// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CapitalizeDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CapitalizeViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="Capitalize"/>.
    /// </summary>
    public class CapitalizeDataViewModel : TransformationDataViewModelBase<Capitalize>
    {
        private CapitalizeMode mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="CapitalizeDataViewModel"/> class.
        /// </summary>
        public CapitalizeDataViewModel()
        {
            this.Exceptions = new ObservableCollection<ExceptionDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        public CapitalizeMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.SetProperty(ref this.mode, value, () => this.Mode);
            }
        }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        public ObservableCollection<ExceptionDataViewModel> Exceptions { get; private set; }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(Capitalize config)
        {
            this.Exceptions.Clear();
            if (config.Exceptions != null)
            {
                foreach (var exception in config.Exceptions)
                {
                    this.Exceptions.Add(new ExceptionDataViewModel { Value = exception });
                }
            }

            this.Mode = config.Mode;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(Capitalize config)
        {
            config.Exceptions = this.Exceptions.Select(e => e.Value).ToArray();
            config.Mode = this.Mode;
        }
    }
}