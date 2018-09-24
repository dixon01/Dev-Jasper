// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTextLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for DynamicTextLayoutElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for DynamicTextLayoutElement.xaml
    /// </summary>
    public partial class DynamicTextLayoutElement
    {
        /// <summary>
        /// The horizontal text alignment
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(System.Windows.HorizontalAlignment),
                typeof(DynamicTextLayoutElement),
                new PropertyMetadata(default(System.Windows.HorizontalAlignment)));

        /// <summary>
        /// The vertical text alignment
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(System.Windows.VerticalAlignment),
                typeof(DynamicTextLayoutElement),
                new PropertyMetadata(default(System.Windows.VerticalAlignment)));

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTextLayoutElement"/> class.
        /// </summary>
        public DynamicTextLayoutElement()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the horizontal text alignment
        /// </summary>
        public System.Windows.HorizontalAlignment HorizontalTextAlignment
        {
            get
            {
                return (System.Windows.HorizontalAlignment)this.GetValue(HorizontalTextAlignmentProperty);
            }

            set
            {
                this.SetValue(HorizontalTextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment
        /// </summary>
        public System.Windows.VerticalAlignment VerticalTextAlignment
        {
            get
            {
                return (System.Windows.VerticalAlignment)this.GetValue(VerticalTextAlignmentProperty);
            }

            set
            {
                this.SetValue(VerticalTextAlignmentProperty, value);
            }
        }
    }
}
