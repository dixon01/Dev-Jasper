// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedImageLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LedRectangleLayoutElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Interaction logic for LedRectangleLayoutElement.xaml
    /// </summary>
    public partial class LedImageLayoutElement
    {
        /// <summary>
        /// The element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(ImageElementDataViewModel),
            typeof(LedImageLayoutElement),
            new PropertyMetadata(default(RectangleElementDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="LedImageLayoutElement"/> class.
        /// </summary>
        public LedImageLayoutElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public ImageElementDataViewModel Element
        {
            get
            {
                return (ImageElementDataViewModel)this.GetValue(ElementProperty);
            }

            set
            {
                this.SetValue(ElementProperty, value);
            }
        }
    }
}
