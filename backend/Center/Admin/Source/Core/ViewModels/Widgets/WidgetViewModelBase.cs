// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WidgetViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Widgets
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The view model base class for widgets on the home stage.
    /// </summary>
    public abstract class WidgetViewModelBase : ViewModelBase
    {
        private string name;

        /// <summary>
        /// Gets or sets the name of the widget used for the widget title.
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