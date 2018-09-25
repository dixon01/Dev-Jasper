// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FrameLayoutElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Interaction logic for FrameLayoutElement.xaml
    /// </summary>
    public partial class FrameLayoutElement
    {
        /// <summary>
        /// The static text element property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element",
            typeof(FrameElementDataViewModel),
            typeof(FrameLayoutElement),
            new PropertyMetadata(default(FrameElementDataViewModel)));

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameLayoutElement"/> class.
        /// </summary>
        public FrameLayoutElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public FrameElementDataViewModel Element
        {
            get
            {
                return (FrameElementDataViewModel)this.GetValue(ElementProperty);
            }

            set
            {
                this.SetValue(ElementProperty, value);
            }
        }
    }
}
