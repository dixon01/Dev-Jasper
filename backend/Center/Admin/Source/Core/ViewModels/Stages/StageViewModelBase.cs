// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StageViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model base class for all stages.
    /// </summary>
    public abstract class StageViewModelBase : ViewModelBase
    {
        private string name;

        /// <summary>
        /// Gets or sets the name of the stage.
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
    }
}