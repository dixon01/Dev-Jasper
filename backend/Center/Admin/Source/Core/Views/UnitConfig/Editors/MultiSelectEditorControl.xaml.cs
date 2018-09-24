// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectEditorControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MultiSelectEditorControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Editors
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MultiSelectEditorControl.xaml
    /// </summary>
    public partial class MultiSelectEditorControl
    {
        /// <summary>
        /// The item image property.
        /// </summary>
        public static readonly DependencyProperty ItemImageProperty = DependencyProperty.Register(
            "ItemImage",
            typeof(ImageSource),
            typeof(MultiSelectEditorControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectEditorControl"/> class.
        /// </summary>
        public MultiSelectEditorControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the image shown next to every item.
        /// </summary>
        public ImageSource ItemImage
        {
            get
            {
                return (ImageSource)this.GetValue(ItemImageProperty);
            }

            set
            {
                this.SetValue(ItemImageProperty, value);
            }
        }
    }
}
