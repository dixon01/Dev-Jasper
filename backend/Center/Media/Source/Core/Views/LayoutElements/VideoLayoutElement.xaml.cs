// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for VideoLayoutElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Interaction logic for VideoLayoutElement.xaml
    /// </summary>
    public partial class VideoLayoutElement
    {
        /// <summary>
        /// The static text element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(VideoElementDataViewModel),
            typeof(VideoLayoutElement),
            new PropertyMetadata(default(VideoElementDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoLayoutElement"/> class.
        /// </summary>
        public VideoLayoutElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public VideoElementDataViewModel Element
        {
            get
            {
                return (VideoElementDataViewModel)this.GetValue(ElementProperty);
            }

            set
            {
                this.SetValue(ElementProperty, value);
            }
        }
    }
}
