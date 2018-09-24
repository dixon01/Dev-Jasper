// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tab.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Tab type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceBench.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the common properties for tabs.
    /// </summary>
    public abstract class Tab : ViewModelBase
    {
        /// <summary>
        /// Gets the description of the tab.
        /// </summary>
        public abstract object Description { get; }

        /// <summary>
        /// Gets the header of the tab.
        /// </summary>
        public abstract object Header { get; }
    }
}
