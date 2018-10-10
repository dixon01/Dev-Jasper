// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomIndicator.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ZoomIndicator.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ZoomIndicator.xaml
    /// </summary>
    public partial class ZoomIndicator
    {
        /// <summary>
        /// the dependency property for the zoom
        /// </summary>
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom",
            typeof(double),
            typeof(ZoomIndicator),
            new PropertyMetadata(100d));

        private const double ZoomStep = 10;

        private const double MaxZoom = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomIndicator"/> class.
        /// </summary>
        public ZoomIndicator()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the zoom percentage
        /// </summary>
        public double Zoom
        {
            get
            {
                return (double)this.GetValue(ZoomProperty);
            }

            set
            {
                this.SetValue(ZoomProperty, value);
            }
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            if (this.Zoom + ZoomStep < MaxZoom)
            {
                this.Zoom += ZoomStep;
            }
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (this.Zoom - ZoomStep > 0)
            {
                this.Zoom -= ZoomStep;
            }
        }
    }
}
