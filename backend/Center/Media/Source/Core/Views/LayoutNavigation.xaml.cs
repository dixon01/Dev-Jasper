// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutNavigation.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LayoutNavigation.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// Interaction logic for LayoutNavigation.xaml
    /// </summary>
    public partial class LayoutNavigation
    {
        /// <summary>
        /// the layout property
        /// </summary>
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
            "Layout",
            typeof(LayoutConfigDataViewModelBase),
            typeof(LayoutNavigation),
            new PropertyMetadata(default(LayoutConfigDataViewModelBase)));

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutNavigation"/> class.
        /// </summary>
        public LayoutNavigation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the current layout
        /// </summary>
        public LayoutConfigDataViewModelBase Layout
        {
            get
            {
                return (LayoutConfigDataViewModelBase)this.GetValue(LayoutProperty);
            }

            set
            {
                this.SetValue(LayoutProperty, value);
            }
        }
    }
}
