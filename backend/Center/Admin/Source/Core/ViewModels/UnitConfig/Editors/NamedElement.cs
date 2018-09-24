// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamedElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// An element inside a <see cref="NamedListEditorViewModel"/>.
    /// </summary>
    public class NamedElement : ViewModelBase
    {
        private string name = string.Empty;

        /// <summary>
        /// Gets or sets the name of this element that is shown.
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
        /// Gets or sets the navigation command used in the first column of the list.
        /// </summary>
        public ICommand NavigateToCommand { get; set; }
    }
}