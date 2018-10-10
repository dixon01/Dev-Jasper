// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolDividerDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolDividerDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The view model for <see cref="SymbolDivider"/>.
    /// </summary>
    public class SymbolDividerDataViewModel : TransformationDataViewModelBase<SymbolDivider>
    {
        private string symbol;

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string Symbol
        {
            get
            {
                return this.symbol;
            }

            set
            {
                this.SetProperty(ref this.symbol, value, () => this.Symbol);
            }
        }

        /// <summary>
        /// Loads all properties from the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to load the properties from.
        /// </param>
        protected override void LoadFrom(SymbolDivider config)
        {
            this.Symbol = config.Symbol;
        }

        /// <summary>
        /// Saves all properties to the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The config to save the properties to.
        /// </param>
        protected override void SaveTo(SymbolDivider config)
        {
            config.Symbol = this.Symbol;
        }
    }
}