// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicDataItemValueProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicDataItemValueProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    /// <summary>
    /// <see cref="IDataItemValueProvider"/> implementation that can change
    /// and provides an initial value given in the constructor.
    /// </summary>
    public sealed class DynamicDataItemValueProvider : DataItemValueProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDataItemValueProvider"/> class.
        /// </summary>
        /// <param name="value">
        /// The initial value to be returned by this provider.
        /// </param>
        public DynamicDataItemValueProvider(string value)
        {
            this.Value = value;
        }
    }
}