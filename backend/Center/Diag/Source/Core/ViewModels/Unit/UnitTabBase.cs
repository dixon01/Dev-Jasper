// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTabBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitTabBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.App;

    /// <summary>
    /// Base class for all tabs representing a unit and the main tab "All Units".
    /// </summary>
    public abstract class UnitTabBase : ViewModelBase
    {
        private string name;

        private RemoteAppViewModel selectedApplication;

        /// <summary>
        /// Gets or sets the name of the unit.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the selected application
        /// </summary>
        public RemoteAppViewModel SelectedApplication
        {
            get
            {
                return this.selectedApplication;
            }

            set
            {
                this.SetProperty(ref this.selectedApplication, value, () => this.SelectedApplication);
            }
        }
    }
}