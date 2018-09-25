// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitRepresentation.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UnitRepresentation.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls
{
    using System.Windows;

    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// Interaction logic for UnitRepresentation.xaml
    /// </summary>
    public partial class UnitRepresentation
    {
        /// <summary>
        /// The unit property
        /// </summary>
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            "Unit",
            typeof(UnitViewModelBase),
            typeof(UnitRepresentation),
            new PropertyMetadata(default(UnitViewModelBase)));

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRepresentation" /> class.
        /// </summary>
        public UnitRepresentation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the unit
        /// </summary>
        public UnitViewModelBase Unit
        {
            get
            {
                return (UnitViewModelBase)this.GetValue(UnitProperty);
            }

            set
            {
                this.SetValue(UnitProperty, value);
            }
        }
    }
}
