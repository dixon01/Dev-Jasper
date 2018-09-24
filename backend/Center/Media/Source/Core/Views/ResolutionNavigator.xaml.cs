// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionNavigator.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ResolutionNavigator.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// Interaction logic for ResolutionNavigator.xaml
    /// </summary>
    public partial class ResolutionNavigator
    {
        /// <summary>
        /// the physical screen property
        /// </summary>
        public static readonly DependencyProperty PhysicalScreenProperty = DependencyProperty.Register(
            "PhysicalScreen",
            typeof(PhysicalScreenConfigDataViewModel),
            typeof(ResolutionNavigator),
            new PropertyMetadata(default(PhysicalScreenConfigDataViewModel)));

        /// <summary>
        /// The virtual display property
        /// </summary>
        public static readonly DependencyProperty VirtualDisplayProperty = DependencyProperty.Register(
            "VirtualDisplay",
            typeof(VirtualDisplayConfigDataViewModel),
            typeof(ResolutionNavigator),
            new PropertyMetadata(default(VirtualDisplayConfigDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionNavigator"/> class.
        /// </summary>
        public ResolutionNavigator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the physical screen
        /// </summary>
        public PhysicalScreenConfigDataViewModel PhysicalScreen
        {
            get
            {
                return (PhysicalScreenConfigDataViewModel)this.GetValue(PhysicalScreenProperty);
            }

            set
            {
                this.SetValue(PhysicalScreenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Virtual Display
        /// </summary>
        public VirtualDisplayConfigDataViewModel VirtualDisplay
        {
            get
            {
                return (VirtualDisplayConfigDataViewModel)this.GetValue(VirtualDisplayProperty);
            }

            set
            {
                this.SetValue(VirtualDisplayProperty, value);
            }
        }
    }
}
