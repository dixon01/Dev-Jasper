// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedRectangleLayoutElement.xaml.cs" company="Gorba AG">
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
    public partial class LedRectangleLayoutElement
    {
        /// <summary>
        /// The element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(RectangleElementDataViewModel),
            typeof(LedRectangleLayoutElement),
            new PropertyMetadata(default(RectangleElementDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="LedRectangleLayoutElement"/> class.
        /// </summary>
        public LedRectangleLayoutElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public RectangleElementDataViewModel Element
        {
            get
            {
                return (RectangleElementDataViewModel)this.GetValue(ElementProperty);
            }

            set
            {
                this.SetValue(ElementProperty, value);
            }
        }
    }
}
