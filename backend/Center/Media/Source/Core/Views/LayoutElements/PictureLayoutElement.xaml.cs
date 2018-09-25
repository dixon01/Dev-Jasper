// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PictureLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for PictureLayoutElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Interaction logic for PictureLayoutElement.xaml
    /// </summary>
    public partial class PictureLayoutElement
    {
        /// <summary>
        /// The static text element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(ImageElementDataViewModel),
            typeof(PictureLayoutElement),
            new PropertyMetadata(default(ImageElementDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureLayoutElement"/> class.
        /// </summary>
        public PictureLayoutElement()
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
