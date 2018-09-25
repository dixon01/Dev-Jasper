// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoSectionViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfoSectionViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The base class for all view models representing an information section of a unit.
    /// </summary>
    public abstract class InfoSectionViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoSectionViewModelBase"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        protected InfoSectionViewModelBase(UnitViewModelBase unit)
        {
            this.Unit = unit;
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IDiagShell Shell
        {
            get
            {
                return this.Unit.Shell;
            }
        }

        /// <summary>
        /// Gets the unit.
        /// </summary>
        public UnitViewModelBase Unit { get; private set; }
    }
}