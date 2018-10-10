// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticDataItemValueProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StaticDataItemValueProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// <see cref="IDataItemValueProvider"/> implementation that never changes
    /// and just provides a value given in the constructor.
    /// </summary>
    public sealed class StaticDataItemValueProvider : DataItemValueProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticDataItemValueProvider"/> class.
        /// </summary>
        /// <param name="value">
        /// The value to be returned by this provider.
        /// </param>
        public StaticDataItemValueProvider(string value)
        {
            base.Value = value;
        }

        /// <summary>
        /// Gets the value from this provider.
        /// </summary>
        public override string Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                throw new NotSupportedException("Can't change the value of a static data item value provider");
            }
        }
    }
}
