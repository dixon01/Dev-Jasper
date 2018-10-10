// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllUnitsTab.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AllUnitsTab type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    /// <summary>
    /// The first tab always showing all units.
    /// </summary>
    public class AllUnitsTab : UnitTabBase
    {
        private UnitViewModelBase selectedUnit;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllUnitsTab"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        public AllUnitsTab(IDiagShell shell)
        {
            this.Shell = shell;
        }

        /// <summary>
        /// Gets or sets the shell
        /// </summary>
        public IDiagShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the currently selected unit
        /// </summary>
        public UnitViewModelBase SelectedUnit
        {
            get
            {
                return this.selectedUnit;
            }

            set
            {
                this.SetProperty(ref this.selectedUnit, value, () => this.SelectedUnit);
            }
        }
    }
}