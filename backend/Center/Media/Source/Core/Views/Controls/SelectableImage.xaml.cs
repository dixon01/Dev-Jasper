// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectableImage.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectableImage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TextBoxWithChangeIndicator.xaml
    /// </summary>
    public partial class SelectableImage
    {
        /// <summary>
        /// the dependency filename property
        /// </summary>
        public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register(
            "Filename", typeof(string), typeof(SelectableImage), new PropertyMetadata(default(string)));

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableImage" /> class.
        /// </summary>
        public SelectableImage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the filename
        /// </summary>
        public string Filename
        {
            get
            {
                return (string)this.GetValue(FilenameProperty);
            }

            set
            {
                this.SetValue(FilenameProperty, value);
            }
        }
    }
}
